using Serilog;
using Serilog.Events;

namespace Api.Startup.SerilogSetup
{
    public static class SerilogSetup
    {
        public static WebApplicationBuilder AddSerilogSetup(this WebApplicationBuilder builder)
        {

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .ReadFrom.Configuration(builder.Configuration)
                .CreateBootstrapLogger();

            builder.Host.UseSerilog((context, services, loggerConfiguration) =>
            {
                loggerConfiguration
                    .ReadFrom.Configuration(context.Configuration)
                    .ReadFrom.Services(services)
                    .Enrich.FromLogContext();

                if (context.HostingEnvironment.IsDevelopment())
                {
                    loggerConfiguration.MinimumLevel.Debug();
                }
                else
                {
                    loggerConfiguration.MinimumLevel.Information();
                }
            });

            return builder;
        }

        public static IApplicationBuilder UseSerilogRequestLoggingSetup(this IApplicationBuilder app)
        {
            app.UseSerilogRequestLogging(options =>
            {
                options.MessageTemplate =
                    "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";

                options.GetLevel = (httpContext, elapsed, ex) =>
                {
                    return ex != null
                        ? LogEventLevel.Error
                        : httpContext.Response.StatusCode >= 500
                            ? LogEventLevel.Error
                            : httpContext.Response.StatusCode >= 400
                                ? LogEventLevel.Warning
                                : LogEventLevel.Information;
                };
            });

            return app;
        }
    }
}