using Microsoft.AspNetCore.Mvc;

namespace MShop.API.Cart.Configuration
{
    public static class ConfigurationModelState
    {
        public static IServiceCollection AddConfigurationModelState(this IServiceCollection services)
        {
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true; // Disable the default model state validation filter
            });

            return services;
        }
    }
}
