using Mshop.Core.DomainObject;
using Mshop.Core.Message;
using Mshop.Core.Message.DomainEvent;
using Mshop.Domain.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mshop.Application.Event.Order
{
    public class OrderCreateEventHandler : IDomainEventHandler<OrderCreateEvent>
    {
        private readonly INotification _notification;
        private readonly IMessageProducer _messageProducer;

        public OrderCreateEventHandler(INotification notification, IMessageProducer messageProducer)
        {
            _notification = notification;
            _messageProducer = messageProducer;
        }

        public async Task<bool> HandlerAsync(OrderCreateEvent domainEvent)
        {
            if (domainEvent.Id == Guid.Empty)
            {
                _notification.AddNotifications("Order ID cannot be empty.");
                return false;
            }

            await _messageProducer.SendMessageAsync(domainEvent);
            return true;
        }
    }
}
