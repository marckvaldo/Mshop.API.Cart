using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MShop.Broker.Cart.Configuration.RabbitMQ
{
    public class RabbitMQConfiguration
    {
        public const string ConfigurationSection = "RabbitMQ";
        public string HostName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Exchange { get; set; }
        public int Port { get; set; }
        public string Vhost { get; set; }
        public string QueueName { get; set; }
        public bool Durable { get; set; }
    }
}
