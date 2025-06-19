using Mshop.Core.DomainObject;
using Mshop.Core.Message.DomainEvent;

namespace Mshop.Application.Event.Order
{
    public class OrderModifiedEventHandler : IDomainEventHandler<DomainEvent>
    {
        public Task<bool> HandlerAsync(DomainEvent domainEvent)
        {
            //aqui eu colocaria a logica de persistir em um banco tipo EventSourcingDb ou MongoB
            throw new NotImplementedException();
        }

    }
}
