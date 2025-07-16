using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Bson;
using MongoDB.Driver;
using Mshop.Infra.Data.Context;
using Mshop.Infra.Data.Interface;
using Mshop.Infra.Data.Repository;
using System.Data;
using System.Data.Common;

namespace Mshop.API.Cart.HealChecks
{
    public class HealthDataBase : IHealthCheck
    {
        protected readonly string _connectionString;
        protected readonly string _dataBase;

        public HealthDataBase(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("MongoDb");
            _dataBase = configuration.GetConnectionString("MshopDb");
        }
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var client = new MongoClient(_connectionString);
                var db = client.GetDatabase(_dataBase);

                // Ping no banco
                db.RunCommandAsync((Command<BsonDocument>)"{ping:1}", cancellationToken: cancellationToken).Wait();

                return HealthCheckResult.Healthy();
            }
            catch (Exception ex)
            {
                return await Task.FromResult(new HealthCheckResult(
                         status: HealthStatus.Unhealthy,
                         description: ex.Message.ToString()
                         ));
            }
        }

        
    }
}
