using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace MShop.Broker.Cart.Configuration.RabbitMQ
{
    public class ChannelManager
    {
        private readonly IConnection _connection;
        private Task<IChannel> _channel;
        private readonly object _lock = new();

        public ChannelManager(IConnection connection)
        {
            _connection = connection;
        }
        public Task<IChannel> GetChannel()
        {
            lock (_lock)
            {
                if (_channel == null || _channel.IsCanceled)
                {
                    _channel = _connection.CreateChannelAsync();
                }
                return _channel;
            }
        }

        public void Dispose()
        {
            _channel?.Dispose();
        }

    }
}
