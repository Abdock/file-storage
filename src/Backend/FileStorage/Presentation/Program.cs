using Presentation.Constants;
using Presentation.Extensions;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args)
    .ConfigureSecrets()
    .ConfigureAuth()
    .ConfigureZLogger()
    .ConfigureDbContext()
    .ConfigureHashing()
    .ConfigureCqrs()
    .ConfigureCors()
    .ConfigureControllers();
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseCustomExceptionHandler();
app.UseCors(EnvironmentConstants.DefaultCorsPolicy);
app.UseAuthentication();
app.UseAuthorization();
app.UseHttpLogging();
app.UseZLoggerContextEnrich();
app.MapControllers();
app.Run();