using Microsoft.Extensions.Options;
using Mshop.Core.Message.DomainEvent;
using MShop.Broker.Cart.Configuration.RabbitMQ;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MShop.Broker.Cart.Producer
{
    public class RabbitMQProducer : IMessageProducer
    {
        private readonly IChannel _channel;
        private readonly string _exchenge;

        public RabbitMQProducer(IChannel channel, IOptions<RabbitMQConfiguration> options)
        {
            _channel = channel;
            _exchenge = options.Value.Exchange;

           
        }

        public async Task SendMessageAsync<T>(T message)
        {

            var routingKey = EventsMapping.GetRoutingKey(message.GetType().Name);
            //para evitar cicles ou loops no relacionamentos quando houver
            var messageBytes = JsonSerializer.SerializeToUtf8Bytes(message,
                new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.IgnoreCycles,
                });

            var channelOpts = new CreateChannelOptions(
                publisherConfirmationsEnabled: true,
                publisherConfirmationTrackingEnabled: true
            //outstandingPublisherConfirmationsRateLimiter: new ThrottlingRateLimiter(MAX_OUTSTANDING_CONFIRMS)
            );

            var props = new BasicProperties
            {
                Persistent = true,
                ContentType = "application/json",
            };

            try
            {
                await _channel.BasicPublishAsync(
                    exchange: _exchenge,
                    routingKey: routingKey,
                    mandatory: true,
                    basicProperties: props,
                    body: messageBytes
                );
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                Console.WriteLine($"Error publishing message: {ex.Message}");

            }
        }
    }
}
