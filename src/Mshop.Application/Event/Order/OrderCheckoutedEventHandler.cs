using Mshop.Core.Message;
using Mshop.Core.Message.DomainEvent;
using Mshop.Domain.Event;

namespace Mshop.Application.Event.Order
{
    public class OrderCheckoutedEventHandler : IDomainEventHandler<OrderCheckoutedEvent>
    {
        private readonly INotification _notification;
        private readonly IMessageProducer _messageProducer;

        public OrderCheckoutedEventHandler(INotification notification, IMessageProducer messageProducer)
        {
            _notification = notification;
            _messageProducer = messageProducer;
        }

        public async Task<bool> HandlerAsync(OrderCheckoutedEvent domainEvent)
        {
            if (domainEvent.Id == Guid.Empty)
            {
                Console.WriteLine("Order ID cannot be empty.");
                return false;
            }

            await _messageProducer.SendMessageAsync(domainEvent);
            return true;
        }
    }
}
