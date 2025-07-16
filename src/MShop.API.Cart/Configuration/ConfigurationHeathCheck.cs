using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Mshop.API.Cart.HealChecks;

namespace Mshop.API.Cart.Configuration
{
    public static class ConfigurationHeathCheck
    {
        public static IServiceCollection AddConfigurationHealthChecks(this IServiceCollection services)
        {
            services.AddHealthChecks()
                .AddCheck<HealthDataBase>("DataBase")
                .AddCheck<HealthCache>("Cache");

            return services;
        }

        public static WebApplication AddMapHealthCheck(this WebApplication app)
        {
            app.MapHealthChecks("/_metrics", new HealthCheckOptions
            {
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
            return app;
        }
    }
}
