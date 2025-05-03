using Mshop.Core.DomainObject;
using Mshop.Domain.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mshop.Domain.Entity
{
    public class Cart : Core.DomainObject.Entity, IAggregateRoot
    {        

        public List<Product> Products { get; private set; }
        public List<Payment> Payments { get; private set; } 
        public Customer Customer { get; private set; }
        public DateTime CreatedAt { get; private set; } 
        public DateTime UpdatedAt { get; private set; }
        public CartStatus Status { get; private set; }
        public string Version { get; private set; } = "1.0.0";

        private List<string> _notifications { get; set; }

        public Cart(Guid id)
        {
            Products = new List<Product>();
            Payments = new List<Payment>();

            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
            
            if(id != Guid.Empty)
                AddId(id);

            Status = CartStatus.PendingCheckout;
            _notifications = new List<string>();

        }

        public Cart()
        {
            _notifications = new List<string>();
        }

       
        public void AddItem(Product product, int quantity)
        {
            if (quantity < 0)
                quantity = 1;

            if (product == null)
                return;

            var existingItem = Products.FirstOrDefault(i => i.Id == product.Id);
            if (existingItem != null)
            {
                existingItem.UpdateQuantity(existingItem.Quantity + quantity);
            }
            else
            {
                product.UpdateQuantity(quantity);
                Products.Add(product);
            }
            UpdatedAt = DateTime.UtcNow;
        }

        public void RemoveQuantity(Guid productId)
        {
            var item = Products.FirstOrDefault(i => i.Id == productId);
            if (item != null)
            {
                if(item.Quantity == 1)
                {
                    Products.Remove(item);
                }
                else 
                    item.UpdateQuantity(item.Quantity - 1);
            }

            UpdatedAt = DateTime.UtcNow;
        }

        public void ClearCart()
        {
            Products.Clear();
            UpdatedAt = DateTime.UtcNow;
        }

        public void RemoveItem(Guid productId)
        {
            var item = Products.FirstOrDefault(i => i.Id == productId);
            if (item != null)
            {
                Products.Remove(item);
                UpdatedAt = DateTime.UtcNow;
            }
        }
        
        public IEnumerable<Product> GetItems()
        {
            return Products;
        }

        public decimal GetTotal()
        {
            return Products.Sum(i => i.GetTotal());
        }

        public decimal GetAmount()
        {
            return Payments.Sum(i => i.Amount);
        }

        public bool UpdateCustomer(Customer customer)
        {
            if(customer == null)
                return false;

            Customer = customer;
            UpdatedAt = DateTime.UtcNow;

            return true;
        }

        public bool AddPayment(Payment payment)
        {
            if(payment == null)
                return false;

            Payments.Add(payment);
            UpdatedAt = DateTime.UtcNow;

            return true;
        }

        public bool IsValid(Core.Message.INotification notification)
        {
            if(Id == Guid.Empty)
                notification.AddNotifications("Carrinho de compras invalido");


            if (Status == CartStatus.CheckoutCompleted)
            {
                if (Payments.Count == 0)
                    notification.AddNotifications("Não é possivel ter um status CheckoutCompleted sem pagamento");


                if(Customer is null)
                    notification.AddNotifications("Não é possivel ter um status CheckoutCompleted sem cliente");

            }

            if (Customer != null)
                Customer.IsValid(notification);


            if(Products.Count > 0)
            {
                Products.ForEach(x =>
                {
                    x.IsValid(notification);
                });
            }

            if (Payments.Count > 0)
            {
                Payments.ForEach(x =>
                {
                    x.IsValid(notification);
                });
            }

            if(GetAmount() > 0 && GetTotal() != GetAmount())
                notification.AddNotifications($"Valor total do carrinho {GetTotal()} e diferente do valor total dos pagamentos {GetAmount()}");

            _notifications.ForEach(x =>
            {
                notification.AddNotifications(x);
            });

            return !notification.HasErrors();

        }

        public bool UpdateStatus(CartStatus status)
        {
            if(Status == CartStatus.CheckoutCompleted)
            {
                _notifications.Add("Não é possivel alterar o status do carrinho pois ja foi feito o chekout");
                return false;
            }

            if(status == CartStatus.CheckoutCompleted && Payments.Count == 0)
            {
                _notifications.Add("Não é possivel alterar o status do carrinho para CheckoutCompleted pois não existe pagamento");
                return false;
            }

            if(status == CartStatus.CheckoutCompleted && GetTotal() != GetAmount())
            {
                _notifications.Add("Valor total do carrinho e diferente do valor total dos pagamentos");
                return false;
            }

            if(Customer is null)
            {
                _notifications.Add("por favor informar um cliente para fazer o checkout");
                return false;
            }

            Status = status;
            UpdatedAt = DateTime.UtcNow;
            return true;
        }

    }

    public enum CartStatus
    {
        CheckoutCompleted,     // Pagamento aguardando processamento
        PendingCheckout,   // Pagamento concluído
    }
}
