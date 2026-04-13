using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Api.Startup.Swagger
{
    public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
    {
        private readonly IApiVersionDescriptionProvider _provider;

        private const string SwaggerIntro = """
**Authentication Requirement – Important**

**This API issue tokens.**
You **must** first authenticate with your received token, then use that token here.

### Quick Instructions
1. Run login api ** (login endpoint).
2. Authenticate using your credentials.
3. Copy the returned JWT token.
4. Paste the token (without the 'Bearer ' prefix) into the value field and confirm.
5. If you donot have login credentials, you would need to register first. Use your desired email and password to register.

""";

        private const string DeprecatedNotice = """
**This API version has been deprecated.** Please use the latest available version.

""";



        public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider)
        {
            _provider = provider;
        }

        public void Configure(SwaggerGenOptions options)
        {
            foreach (var description in _provider.ApiVersionDescriptions)
            {
                var groupName = description.GroupName;

                var docDescription =
                    SwaggerIntro
                    + (description.IsDeprecated ? DeprecatedNotice : string.Empty)
                    + "API for Products";

                options.SwaggerDoc(groupName, new OpenApiInfo
                {
                    Title = "Products API Service - zenteklabs",
                    Version = description.ApiVersion.ToString(),
                    Description = docDescription
                });
            }
        }

        public void Configure(string? name, SwaggerGenOptions options) => Configure(options);
    }
}
