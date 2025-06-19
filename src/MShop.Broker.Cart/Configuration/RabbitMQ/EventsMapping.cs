using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MShop.Broker.Cart.Configuration.RabbitMQ
{
    public class EventsMapping
    {
        private static Dictionary<string, string> _routingKey => new()
        {
            {"OrderCheckoutedEvent", "Order.v1.Checkout" },
        };

        public static string GetRoutingKey(string eventName) => _routingKey[eventName];
    }
}
