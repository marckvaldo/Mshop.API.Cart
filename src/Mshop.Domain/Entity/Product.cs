using Mshop.Domain.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mshop.Domain.Entity
{
    public class Product : Core.DomainObject.Entity
    {

        public string Description { get; private set; }

        public string Name { get; private set; }

        public decimal Price { get; private set; }

        public decimal Total { get; private set; }

        public bool IsSale { get; private set; }

        public Guid CategoryId { get; private set; }

        public decimal Quantity { get; private set; }

        public string Category { get; private set; }

        public string? Thumb { get; private set; }


        public Product(Guid id, string description, string name, decimal price, bool isSale, 
            Guid categoryId, string category, string? thumb, decimal quantity)
        {
            Description = description;
            Name = name;
            Price = price;
            IsSale = isSale;
            CategoryId = categoryId;
            Category = category;
            Thumb = thumb;
            Quantity = quantity;

            if (id != Guid.Empty)
                AddId(id);

            Totalize();
        }

        public bool IsValid(Core.Message.INotification notification)
        {
            var validator = new ProductValidation();
            var validationResult = validator.Validate(this);
            validationResult.Errors.ToList().ForEach(x =>
            {
                notification.AddNotifications(x.ErrorMessage);
            });
            return validationResult.IsValid;
        }

        public void UpdateQuantity(decimal quantity)
        {
            Quantity = quantity;
            Totalize();
        }

        public decimal GetTotal()
        {
            Totalize();
            return Total;
        }

        private void Totalize()
        {
            Total = Price * Quantity;
        }

    }
}
