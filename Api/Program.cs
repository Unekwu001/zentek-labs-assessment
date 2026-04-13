using Api.ProgramSetup.DI;
using Asp.Versioning.ApiExplorer;


var builder = WebApplication.CreateBuilder(args);

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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
