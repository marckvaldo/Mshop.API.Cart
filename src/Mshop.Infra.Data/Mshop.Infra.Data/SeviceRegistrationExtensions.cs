using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Mshop.Infra.Data.Context;
using Mshop.Infra.Data.Interface;
using Mshop.Infra.Data.Repository;

namespace Mshop.Infra.Data
{
    public static class SeviceRegistrationExtensions
    {
        public static IServiceCollection AddDataBaseAndRepository(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration["ConnectionStrings:MongoDb"];
            var dataBase = configuration["ConnectionStrings:DatabaseName"];

            Mapping.MapConfiguration();

            services.AddSingleton(new MongoDbContext(connectionString, dataBase));
            services.AddScoped<IMongoDatabase>(sp => sp.GetRequiredService<MongoDbContext>().Database);

            services.AddScoped<StartIndex.StartIndex>();
            services.AddScoped<ICartRepository, CartRepository>();
            

            return services;
        }

        public static WebApplication CreateIndexMongoDB(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var mongoDBService = scope.ServiceProvider.GetRequiredService<StartIndex.StartIndex>();
            mongoDBService.CreateIndexes().Wait();
            return app; 
        }

    }
}
