using Mshop.Domain.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mshop.Domain.Entity
{
    public class Customer : Core.DomainObject.Entity
    {
        public string Name { get; private set; }
        public string Email { get; private set; }
        public string Phone { get; private set; }

        public Address Address { get; private set; }

        public Customer(Guid id, string name, string email, string phone)
        {
            Name = name;
            Email = email;
            Phone = phone;

            if (id != Guid.Empty)
                AddId(id);
        }

        public bool IsValid(Core.Message.INotification notification)
        {
            var validator = new CustomerValidation();
            var validationResult = validator.Validate(this);
            validationResult.Errors.ToList().ForEach(x =>
            {
                notification.AddNotifications(x.ErrorMessage);
            });
            return validationResult.IsValid;
        }

        public void AddAdress(Address adress)
        {
            Address = adress;
        }

    }
}
