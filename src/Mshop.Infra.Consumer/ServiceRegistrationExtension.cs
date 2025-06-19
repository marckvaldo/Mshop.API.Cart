using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Mshop.Infra.Consumer.Cache;
using Mshop.Infra.Consumer.CircuitBreakerPolicy;
using Mshop.Infra.Consumer.GRPC;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mshop.Infra.Consumer
{
    public static class ServiceRegistrationExtension
    {
        public static IServiceCollection AddConsumer(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<ICircuitBreaker, CircuitBreaker>();

            services.AddScoped<IServiceCache, ServiceCache>(opt =>
            {
                var redis = opt.GetRequiredService<IConnectionMultiplexer>();
                return new ServiceCache(redis);
            });

            services.AddScoped<IServicerGRPC, ServiceGRPC>(opt =>
            {
                var settings = new SettingsGRPC
                {
                    GrpcProduct = configuration["Urls:GrpcProduct"],
                    GrpcCustomer = configuration["Urls:GrpcCustomer"],
                    TimeoutInSeconds = 10
                };

                var circuitBreaker = opt.GetRequiredService<ICircuitBreaker>();
                var options = Options.Create(settings);
                return new ServiceGRPC(options, circuitBreaker);

            });

            return services;
        }

        public static IServiceCollection AddCache(this IServiceCollection services, IConfiguration configuration)
        {
            try
            {
                var redisPassword = configuration["Redis:Password"];
                var redisEndPoint = configuration["Redis:Endpoint"];
                var redisUser = configuration["Redis:User"];

                var conf = new ConfigurationOptions
                {
                    EndPoints = { redisEndPoint },
                    Password = redisPassword,
                    User = redisUser
                };

                var redis = ConnectionMultiplexer.Connect(conf);
                services.AddSingleton<IConnectionMultiplexer>(redis);

            }
            catch { }

            return services;

        }
    }
}
