using Presentation.Constants;
using Presentation.Extensions;

var builder = WebApplication.CreateBuilder(args)
    .ConfigureSecrets()
    .ConfigureAuth()
    .ConfigureLogging()
    .ConfigureDbContext()
    .ConfigureHashing()
    .ConfigureCqrs()
    .ConfigureCors()
    .ConfigureControllers();
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors(EnvironmentConstants.DefaultCorsPolicy);
app.UseCustomExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();
app.UseHttpLogging();
app.UseLogContextEnrich();
app.MapControllers();
app.Run();