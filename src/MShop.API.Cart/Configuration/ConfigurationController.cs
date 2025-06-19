using MShop.API.Cart.Filter;
using System.Text.Json.Serialization;

namespace MShop.API.Cart.Configuration
{
    public static class ConfigurationController
    {
        public static IServiceCollection AddConfigurationController(this IServiceCollection services)
        {
            services.AddControllers(options =>
            {
                options.Filters.Add(typeof(ApiGlobalExceptionFilter));

            }).AddJsonOptions(options => options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

            services.AddEndpointsApiExplorer();
            return services;
        }
    }
}
