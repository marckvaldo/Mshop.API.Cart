using Microsoft.Extensions.Diagnostics.HealthChecks;
using StackExchange.Redis;
using System.Data;
using System.Data.Common;

namespace Mshop.API.Cart.HealChecks
{
    public class HealthCache : IHealthCheck
    {
        protected readonly string _redisPassword;
        protected readonly string _redisEndPoint;
        protected readonly string _redisUser;

        public HealthCache(IConfiguration configuration)
        {
            _redisPassword = configuration["Redis:Password"];
            _redisEndPoint = configuration["Redis:Endpoint"];
            _redisUser = configuration["Redis:User"];
        }
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var configurationOptions = new ConfigurationOptions
                {
                    EndPoints = { _redisEndPoint },
                    Password = _redisPassword,     
                    User = _redisUser              
                };

              
                using (var connection = ConnectionMultiplexer.Connect(configurationOptions))
                {
                    var server = connection.GetServer(_redisEndPoint);
                    if (server.IsConnected)
                        return HealthCheckResult.Healthy("Redis is healthy");
                    else
                        return HealthCheckResult.Unhealthy("Redis is not responding");

                }
            }
            catch (Exception ex)
            {
                return await Task.FromResult(new HealthCheckResult(
                    status: HealthStatus.Unhealthy,
                    description: $"Exception occurred while connecting to Redis: {ex.Message}"
                ));
            }

        }
    }
}
