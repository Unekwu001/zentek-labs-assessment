using Amazon;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using Api.ProgramSetup.DI;
using Api.Startup.DbSetup;
using Api.Startup.SerilogSetup;
using Asp.Versioning.ApiExplorer;
using Serilog;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.AddSerilogSetup();


builder.Services.SetupDependencyInjection(builder.Configuration);
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

    foreach (var description in provider.ApiVersionDescriptions)
    {
        options.SwaggerEndpoint(
            $"/swagger/{description.GroupName}/swagger.json",
            $"Products API {description.GroupName.ToUpperInvariant()}"
        );
    }

    options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
});

app.UseCors("AllowFrontend");
app.UseSerilogRequestLoggingSetup();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.ApplyDatabaseMigrations();

app.Run();
