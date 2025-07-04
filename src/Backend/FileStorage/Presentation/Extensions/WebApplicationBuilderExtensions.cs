using System.Text;
using Application.Options;
using Application.Services.Hashing;
using Application.Services.JWT;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Persistence.Context;
using Presentation.Constants;
using Serilog;
using Serilog.Events;

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
        const string logMessageTemplate = "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {EnvironmentName} {CorrelationId} {Level:u3}] {Username} {UserId} {ClientIp} {Operation} {Message:lj}{NewLine}{Exception}";
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
        return builder;
    }

    public static WebApplicationBuilder ConfigureSecrets(this WebApplicationBuilder builder)
    {
        builder.Configuration.AddUserSecrets<Program>();
        return builder;
    }

    public static WebApplicationBuilder ConfigureControllers(this WebApplicationBuilder builder)
    {
        builder.Services.AddControllers();
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