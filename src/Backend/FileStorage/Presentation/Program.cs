using Presentation.Extensions;

var builder = WebApplication.CreateBuilder(args)
    .ConfigureSecrets()
    .ConfigureAuth()
    .ConfigureLogging()
    .ConfigureDbContext()
    .ConfigureHashing()
    .ConfigureCqrs()
    .ConfigureControllers();
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();