using Mshop.Core.DomainObject;
using Mshop.Core.Message.DomainEvent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mshop.Application.Event.Order
{
    public class OrderItemModifiedEventHandler : IDomainEventHandler<DomainEvent>
    {
        public Task<bool> HandlerAsync(DomainEvent domainEvent)
        {
            //aqui eu colocaria a logica de persistir em um banco tipo EventSourcingDb ou MongoB
            throw new NotImplementedException();
        }

    }
}
