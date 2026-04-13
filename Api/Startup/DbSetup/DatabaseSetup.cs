using Microsoft.EntityFrameworkCore;
using Data.AppDbContext;
using Data.Interceptors;

namespace Api.Startup.DbSetup
{

    public static class DatabaseSetup
    {


        public static IServiceCollection AddApplicationDbContext(
                this IServiceCollection services,
                IConfiguration configuration)
        {
            services.AddSingleton<AuditingAndSoftDeleteInterceptor>();

            services.AddDbContext<AppDbContext>((serviceProvider, options) =>
            {
                var connectionString = configuration.GetConnectionString("DefaultConnection")
                    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

                var auditInterceptor = serviceProvider.GetRequiredService<AuditingAndSoftDeleteInterceptor>();

                options.UseSqlite(connectionString);

                options.AddInterceptors(auditInterceptor);

                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
            });

            return services;
        }



    }



}
