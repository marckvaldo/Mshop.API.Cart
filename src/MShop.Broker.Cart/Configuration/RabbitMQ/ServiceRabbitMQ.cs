using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MShop.Broker.Cart.Configuration.RabbitMQ
{
    public class ServiceRabbitMQ
    {
        private readonly IOptions<RabbitMQConfiguration> _rabbitmqConfiguration;
        private readonly IChannel _channel;

        private readonly string _exchengeOrder;
        private readonly string _nameQueueOrder;
        private readonly string[] _routeKey = { "Order.#" };
        private readonly bool _durable;

        public ServiceRabbitMQ(IOptions<RabbitMQConfiguration> rabbitmqConfiguration, IChannel channel)
        {
            _rabbitmqConfiguration = rabbitmqConfiguration;
            _channel = channel;

            _exchengeOrder = _rabbitmqConfiguration.Value.Exchange;
            _nameQueueOrder = _rabbitmqConfiguration.Value.QueueName;
            _durable = _rabbitmqConfiguration.Value.Durable;
        }

        public async Task SetUp()
        {
            await _channel.ExchangeDeclareAsync(_exchengeOrder, ExchangeType.Topic, _durable, false);
            await _channel.QueueDeclareAsync(_nameQueueOrder, _durable, false, false);
            
            foreach (var routeKey in _routeKey)
            {
                await _channel.QueueBindAsync(_nameQueueOrder, _exchengeOrder, routeKey);
            }
        }

        public async Task SetUpWithDeadLetter()
        {
            var arguments = await DeadLertterQueue();

            await  _channel.ExchangeDeclareAsync(_exchengeOrder, ExchangeType.Topic, _durable, false);
            await  _channel.QueueDeclareAsync(_nameQueueOrder, _durable, false, false, arguments);
            
            foreach (var routeKey in _routeKey)
            {
                await _channel.QueueBindAsync(_nameQueueOrder, _exchengeOrder, routeKey);
            }
        }

        public async Task Down()
        {
            foreach (var routeKey in _routeKey)
            {
                await _channel.QueueUnbindAsync(_nameQueueOrder, _exchengeOrder, routeKey, null);
            }

            await _channel.QueueDeleteAsync(_nameQueueOrder);
            await _channel.ExchangeDeleteAsync(_exchengeOrder);
        }

        public async Task DownDeadLetter()
        {
            var exchengeDead = $"{_exchengeOrder}.DeadLetter";
            var queueDead = $"{_nameQueueOrder}.DeadLetter";

            foreach (var routeKey in _routeKey)
            {
                await _channel.QueueUnbindAsync(queueDead, exchengeDead, routeKey, null);
            }
            await _channel.QueueDeleteAsync(queueDead, false, false);
            await _channel.ExchangeDeleteAsync(exchengeDead, false);
        }


        private async Task<Dictionary<string, object>> DeadLertterQueue()
        {
            var exchengeDead = $"{_exchengeOrder}.DeadLetter";
            var queueDead = $"{_nameQueueOrder}.DeadLetter";

            await _channel.ExchangeDeclareAsync(exchengeDead!, ExchangeType.Topic, _durable, false);
            await _channel.QueueDeclareAsync(queueDead!, _durable, false, false);

            foreach (var routeKey in _routeKey)
            {
                await _channel.QueueBindAsync(queueDead!, exchengeDead!, routeKey, null);
            }

            return new Dictionary<string, object>
            {
                {"x-dead-letter-exchange",exchengeDead}
            };

        }

    }
}
