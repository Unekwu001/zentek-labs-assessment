using Api.Startup.Cors;
using Api.Startup.DbSetup;
using Api.Startup.Jwt;
using Api.Startup.Swagger;
using Asp.Versioning;
using Core.Repos.ProductRepositories;
using Core.Repos.UserRepositories;
using Core.Services.ProductServices;
using Core.Services.UserServices;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Net.Http.Headers;
using System.Text.Json.Serialization;

namespace Api.ProgramSetup.DI
{
    public static class DependencyInjectionSetup
    {
        public static IServiceCollection SetupDependencyInjection(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });
            services.AddEndpointsApiExplorer();

            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
                options.ApiVersionReader = ApiVersionReader.Combine(
                    new UrlSegmentApiVersionReader(),
                    new HeaderApiVersionReader("X-Api-Version"),
                    new MediaTypeApiVersionReader("v")
                );
            })
            .AddMvc()
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });
            services.AddCustomCors();
            services.AddMemoryCache();
            services.AddApplicationDbContext(configuration);
            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
            services.AddScoped<IProductRepo, ProductRepo>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IUserRepo, UserRepo>();
            services.AddScoped<IUserService, UserService>();
            services.AddSwaggerWithJwt();
            services.AddJwtAuthentication(configuration);
            services.AddAuthorization();
            services.AddHttpContextAccessor();


            return services;
        }

    }
}
