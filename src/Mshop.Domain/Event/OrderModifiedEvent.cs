using Mshop.Core.DomainObject;
using Mshop.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mshop.Domain.Event
{
    public class OrderModifiedEvent : DomainEvent
    {
        public OrderModifiedEvent(Cart cart) : base()
        {

            if (cart == null)
                throw new ArgumentNullException(nameof(cart));

            Customer = cart.Customer;
            Payments = cart.Payments;
            Products = cart.Products;
            CreatedAt = cart.CreatedAt;
            UpdatedAt = cart.UpdatedAt;
            Status = cart.Status;
            Id = cart.Id;
        }

        public Customer Customer { get; private set; }
        public List<Payment> Payments { get; private set; }
        public List<Product> Products { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        public CartStatus Status { get; private set; }

    }

}
