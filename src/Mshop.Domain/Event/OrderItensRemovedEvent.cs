using Mshop.Core.DomainObject;
using Mshop.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mshop.Domain.Event
{
    public class OrderItensRemovedEvent : DomainEvent
    {
        public OrderItensRemovedEvent(Cart cart) : base()
        {

            if (cart == null)
                throw new ArgumentNullException(nameof(cart));

            CreatedAt = cart.CreatedAt;
            UpdatedAt = cart.UpdatedAt;
            Status = cart.Status;
            Id = cart.Id;
        }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        public CartStatus Status { get; private set; }

    }

}
