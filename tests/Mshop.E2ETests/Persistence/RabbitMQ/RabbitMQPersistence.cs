using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Mshop.Core.DomainObject;
using Mshop.Domain.Event;
using MShop.Broker.Cart.Configuration.RabbitMQ;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Mshop.E2ETests.Persistence.RabbitMQ
{
    public class RabbitMQPersistence
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IChannel _channel;
        private readonly RabbitMQConfiguration _rabbitMQConfiguration;
        private readonly ServiceRabbitMQ _serviceRabbitMQ;

        public RabbitMQPersistence(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            var ChannelManager = _serviceProvider.GetService<ChannelManager>().GetChannel();
            _channel = ChannelManager.GetAwaiter().GetResult();
            _rabbitMQConfiguration = _serviceProvider.GetRequiredService<IOptions<RabbitMQConfiguration>>().Value;
            _serviceRabbitMQ = _serviceProvider.GetService<ServiceRabbitMQ>();

        }

        public async Task<(TEvent, uint)> ReadMessageFromRabbitMQAutoAck<TEvent>()
        {
            var options = _rabbitMQConfiguration;
            var consumer = await _channel.BasicGetAsync(options.QueueOrder, true);

            if (consumer is null) return (default, 0);

            var body = consumer.Body.ToArray();

            var message = Encoding.UTF8.GetString(body);

            var optionsjson = new JsonSerializerOptions
            {
                IncludeFields = true, // Permite a desserialização de campos.
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase, // Configuração para nomes de propriedades.
            };

            var @event = JsonSerializer.Deserialize<TEvent>(message);

            return (@event, consumer.MessageCount);
        }

        public async void ReadMessageFromRabbitMQNAck<TEvent>(Action<TEvent> action) where TEvent : DomainEvent
        {
            var options = _rabbitMQConfiguration;
            
            var consumer = _channel.BasicGetAsync(options.QueueOrder, false).Result;

            if (consumer is null) return;

            await _channel.BasicNackAsync(consumer.DeliveryTag, false, false);

        }

        public async Task<(TEvent, uint)> ReadMessageFromRabbitMQDeadLetterQueue<TEvent>() where TEvent : DomainEvent
        {
            var options = _rabbitMQConfiguration;

            var QueueName = $"{options.QueueOrder}.DeadLetter";

            var consumer = await _channel.BasicGetAsync(QueueName, true);

            if (consumer is null) return (null, 0);
            var body = consumer.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var @event = JsonSerializer.Deserialize<TEvent>(message);

            return (@event, consumer.MessageCount);
        }

        public async Task SetupRabbitMQWithDeadLette()
        {
             await _serviceRabbitMQ.SetUpWithDeadLetter();
        }

        public async Task DownRabbitMQWithDeadLetter()
        {
            await _serviceRabbitMQ.Down();
            await _serviceRabbitMQ.DownDeadLetter();
        }
    }
}
