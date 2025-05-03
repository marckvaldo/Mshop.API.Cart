using Microsoft.Extensions.Options;
using Mshop.Core.Message.DomainEvent;
using MShop.Broker.Cart.Configuration.RabbitMQ;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public Task SendMessageAsync<T>(T message)
        {
            throw new NotImplementedException();
        }
    }
}
