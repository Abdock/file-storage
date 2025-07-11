using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Application.Options;
using Application.Services.Hashing;
using Application.Services.JWT;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Persistence.Context;
using Presentation.Constants;
using Serilog;
using Serilog.Events;
using ZLinq;
using ZLogger;
using ZLogger.Formatters;
using MessageTemplate = ZLogger.MessageTemplate;

namespace Presentation.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder ConfigureCqrs(this WebApplicationBuilder builder)
    {
        builder.Services.AddMediator();
        return builder;
    }

    public static WebApplicationBuilder ConfigureDbContext(this WebApplicationBuilder builder)
    {
        builder.Services.AddPooledDbContextFactory<StorageContext>(optionsBuilder =>
        {
            optionsBuilder
                .UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
                .EnableSensitiveDataLogging(builder.Environment.IsDevelopment());
        });
        return builder;
    }

    public static WebApplicationBuilder ConfigureHashing(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IHasher, Hasher>();
        return builder;
    }

    public static WebApplicationBuilder ConfigureLogging(this WebApplicationBuilder builder)
    {
        const string logMessageTemplate = "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {EnvironmentName} {CorrelationId} {Level:u3}] User id: {UserId} API Token: {ApiToken} IP: {ClientIp} {Operation} {Message:lj}{NewLine}{Exception}";
        var logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .Enrich.WithEnvironmentName()
            .Enrich.WithClientIp()
            .WriteTo
            .Async(configuration =>
            {
                configuration.Console(restrictedToMinimumLevel: LogEventLevel.Information, outputTemplate: logMessageTemplate);
                configuration.File("logs.log", rollingInterval: RollingInterval.Hour, restrictedToMinimumLevel: LogEventLevel.Information, outputTemplate: logMessageTemplate);
            })
            .CreateLogger();
        builder.Logging.ClearProviders();
        builder.Logging.AddSerilog(logger);
        builder.Services.AddHttpLogging(options =>
        {
            options.LoggingFields = HttpLoggingFields.RequestHeaders;
        });
        return builder;
    }

    public static WebApplicationBuilder ConfigureZLogger(this WebApplicationBuilder builder)
    {
        Action<PlainTextZLoggerFormatter> formatterConfiguration = formatter =>
        {
            formatter.SetPrefixFormatter($"[{0:utc-longdate} {1:short} {2}] {3} User id: {4} API Token: {5} ", (in MessageTemplate template, in LogInfo info) =>
            {
                var scope = info.ScopeState;
                string correlationId = string.Empty, userId = string.Empty, apiToken = string.Empty;
                if (scope is not null && !scope.IsEmpty)
                {
                    var props = scope.Properties.AsValueEnumerable();
                    correlationId = props
                        .FirstOrDefault(e => e.Key.Equals(LoggingConstants.CorrelationIdKey))
                        .Value?.ToString() ?? string.Empty;
                    userId = props
                        .FirstOrDefault(e => e.Key.Equals(LoggingConstants.UserIdKey))
                        .Value?.ToString() ?? string.Empty;
                    apiToken = props
                        .FirstOrDefault(e => e.Key.Equals(LoggingConstants.ApiTokenKey))
                        .Value?.ToString() ?? string.Empty;
                }

                template.Format(info.Timestamp, info.LogLevel, correlationId, info.EventId, userId, apiToken);
            });
            formatter.SetExceptionFormatter((writer, exception) =>
            {
                Utf8StringInterpolation.Utf8String.Format(writer, $"{exception.Message}");
            });
        };
        builder.Logging
            .ClearProviders()
            .AddFilter("Microsoft.AspNetCore", LogLevel.Information)
            .AddFilter("Microsoft.AspNetCore.HttpLogging", LogLevel.Information)
            .AddFilter("Microsoft.AspNetCore.Hosting", LogLevel.Information)
            .AddFilter("Microsoft.AspNetCore.Routing", LogLevel.Debug) 
            .AddZLoggerConsole(options =>
            {
                options.TimeProvider = TimeProvider.System;
                options.IncludeScopes = true;
                options.UsePlainTextFormatter(formatterConfiguration);
            })
            .AddZLoggerRollingFile((options, services) =>
            {
                var environment = services.GetRequiredService<IHostEnvironment>();
                options.FilePathSelector = (offset, sequenceNumber) => Path.Combine(environment.ContentRootPath, "logs", $"zlogs-{offset:yyyyddMM}_{sequenceNumber:000}.log");
                options.TimeProvider = TimeProvider.System;
                options.RollingInterval = ZLogger.Providers.RollingInterval.Day;
                options.IncludeScopes = true;
                options.UsePlainTextFormatter(formatterConfiguration);
            });
        builder.Services.AddHttpLogging(options =>
        {
            options.LoggingFields = HttpLoggingFields.RequestHeaders;
        });
        return builder;
    }

    public static WebApplicationBuilder ConfigureSecrets(this WebApplicationBuilder builder)
    {
        builder.Configuration.AddUserSecrets<Program>();
        return builder;
    }

    public static WebApplicationBuilder ConfigureControllers(this WebApplicationBuilder builder)
    {
        var defaultEnumConverter = new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseUpper);
        builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(defaultEnumConverter);
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            });
        builder.Services.AddOpenApi()
            .ConfigureHttpJsonOptions(options =>
            {
                options.SerializerOptions.Converters.Add(defaultEnumConverter);
                options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            });
        return builder;
    }

    public static WebApplicationBuilder ConfigureAuth(this WebApplicationBuilder builder)
    {
        var section = builder.Configuration.GetRequiredSection(EnvironmentConstants.JwtOptions);
        builder.Services.Configure<JwtOptions>(section);
        var issuer = section["Issuer"]!;
        var audience = section["Audience"]!;
        var securityKey = section["SecurityKey"]!;
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey));
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = key,
                    ValidAlgorithms = [SecurityAlgorithms.HmacSha256],
                    ClockSkew = TimeSpan.Zero, 
                };
            });
        builder.Services.AddAuthorization();
        builder.Services.AddSingleton<IJwtService, JwtService>();
        return builder;
    }

    public static WebApplicationBuilder ConfigureCors(this WebApplicationBuilder builder)
    {
        builder.Services.AddCors(options =>
        {
            options.AddPolicy(EnvironmentConstants.DefaultCorsPolicy, policyBuilder =>
            {
                policyBuilder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });
        return builder;
    }
}