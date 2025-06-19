using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Mshop.Core.Message.DomainEvent;
using MShop.Broker.Cart.Configuration.RabbitMQ;
using MShop.Broker.Cart.Producer;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MShop.Broker.Cart
{
    public static class ServiceRegistrationExtension
    {
        public static IServiceCollection AddRabbitMQ(this IServiceCollection services, IConfiguration configuration)
        { 
            
            //coloca as configurações RabbitMQ nos serviços da minha applicação para recuperar posteriomente
            services.Configure<RabbitMQConfiguration>(
                configuration.GetSection(RabbitMQConfiguration.ConfigurationSection)
                );

            services.AddSingleton<IConnection>(options =>
            {
                RabbitMQConfiguration config = options.GetRequiredService<IOptions<RabbitMQConfiguration>>().Value;

                var factory = new ConnectionFactory()
                {
                    HostName = config.HostName,
                    UserName = config.UserName,
                    Password = config.Password,
                    Port = config.Port,
                    VirtualHost = config.Vhost
                };

                return factory.CreateConnectionAsync().GetAwaiter().GetResult();
            });


            //criar um canal no rabbitMQL
            services.AddSingleton<ChannelManager>();


            //criar as queues 
            services.AddSingleton<ServiceRabbitMQ>(options =>
            {
                var channelManager = options.GetRequiredService<ChannelManager>();
                var config = options.GetRequiredService<IOptions<RabbitMQConfiguration>>();
                var channel = channelManager.GetChannel().GetAwaiter().GetResult();
                var serviceRabbitMQ = new ServiceRabbitMQ(config, channel);
                serviceRabbitMQ.SetUpWithDeadLetter();
                return serviceRabbitMQ;
            });


            services.AddScoped<IMessageProducer>(options =>
            {
                //aqui eu chamo a manager channel
                var channelManager = options.GetRequiredService<ChannelManager>();
                var config = options.GetRequiredService<IOptions<RabbitMQConfiguration>>();
                
                var channel = channelManager.GetChannel().GetAwaiter().GetResult();

                return new RabbitMQProducer(channel, config);
            });

            return services;

        }

    }
}
