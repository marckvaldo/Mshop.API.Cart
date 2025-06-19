using Mshop.Core.DomainObject;
using Mshop.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mshop.Domain.Event
{
    public class OrderItemRemovedEvent : DomainEvent
    {
        public OrderItemRemovedEvent(Cart cart) : base()
        {

            if (cart == null)
                throw new ArgumentNullException(nameof(cart));

            Products = cart.Products;
            CreatedAt = cart.CreatedAt;
            UpdatedAt = cart.UpdatedAt;
            Status = cart.Status;
            Id = cart.Id;
        }

        public List<Product> Products { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        public CartStatus Status { get; private set; }

    }

}
