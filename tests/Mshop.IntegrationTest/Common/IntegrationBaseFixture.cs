using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mshop.Application;
using Mshop.Core.Test.Domain;
using Mshop.Infra.Consumer;
using Mshop.Infra.Data;
using Mshop.Infra.Data.Context;
using Mshop.Infra.Data.StartIndex;
using Mshop.IntegrationTest.Infra.configuration;
using MShop.Broker.Cart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mshop.IntegrationTest.Common
{
    public class IntegrationBaseFixture : DomainEntityFixture
    {
        protected IConfiguration _configuration;

        protected IServiceProvider _serviceProvider;

        public IntegrationBaseFixture()
        {
            _serviceProvider = BuildProviders();
        }

        public IServiceProvider BuildProviders()
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");

            var service = new ServiceCollection();
            
            var inMemorySettings = new Dictionary<string, string>
            {
                {"ConnectionStrings:MongoDB", ConfigurationTests.ConnectionString},
                {"ConnectionStrings:DatabaseName", ConfigurationTests.DataBase},
                {"Urls:GrpcProduct", ConfigurationTests.GrpcProduct },
                {"Urls:GrpcCustomer", ConfigurationTests.GrpcCustomer },
                {"Redis:Endpoint", ConfigurationTests.EndpointCache },
                {"Redis:Password", ConfigurationTests.PasswordCache },
                {"Redis:User", ConfigurationTests.UserCache },
                {"RabbitMQ:HostName", ConfigurationTests.HostNameRabbitMQ },
                {"RabbitMQ:UserName", ConfigurationTests.UserNameRabbitMQ },
                {"RabbitMQ:Password", ConfigurationTests.PasswordRabbitMQ },
                {"RabbitMQ:Exchange", ConfigurationTests.ExchangeRabbitMQ },
                {"RabbitMQ:Port", ConfigurationTests.PortRabbitMQ },
                {"RabbitMQ:Vhost", ConfigurationTests.VhostRabbitMQ },
                {"RabbitMQ:QueueOrder", ConfigurationTests.QueueOrderRabbitMQ },
                {"RabbitMQ:Durable", ConfigurationTests.DurableRabbitMQ.ToString() },
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            Mapping.MapConfiguration();

            service.AddDataBaseAndRepository(configuration);
            service.AddCache(configuration);
            service.AddConsumer(configuration);
            service.AddRabbitMQ(configuration);
            service.AddAplication(configuration);

            _configuration = configuration;
            
            return service.BuildServiceProvider();
        }

        public async Task CreateCollection(IServiceProvider serviceProvider, string CollectionName)
        {
            var startIndex = serviceProvider.GetRequiredService<StartIndex>();
            await startIndex.CrateCollection(CollectionName);
        }
        public async Task DropCollection(IServiceProvider serviceProvider, string CollectionName)
        {
            var startIndex = serviceProvider.GetRequiredService<StartIndex>();
            await startIndex.DropCollection(CollectionName);
        }
    }
}
