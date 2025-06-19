using Mshop.Core.DomainObject;
using Mshop.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mshop.Domain.Event
{
    public class OrderCheckoutedEvent : DomainEvent
    {
        public OrderCheckoutedEvent(Cart cart) :base()
        {

            if (cart == null)
                throw new ArgumentNullException(nameof(cart));

            Products = cart.Products;
            Payments = cart.Payments;
            Customer = cart.Customer;
            CreatedAt = cart.CreatedAt;
            UpdatedAt = cart.UpdatedAt;
            Status = cart.Status;
            Id = cart.Id;
        }

        public List<Product> Products { get; private set; }
        public List<Payment> Payments { get; private set; }
        public Customer Customer { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        public CartStatus Status { get; private set; }
    }

}
