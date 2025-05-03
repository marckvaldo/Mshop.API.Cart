using Mshop.Domain.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mshop.Domain.Entity
{
    public class Address
    {
        public string Street { get; private set; } // Rua
        public string Number { get; private set; } // Número
        public string Complement { get; private set; } // Complemento (opcional)
        public string District { get; private set; } // Bairro
        public string City { get; private set; } // Cidade
        public string State { get; private set; } // Estado
        public string PostalCode { get; private set; } // CEP
        public string Country { get; private set; } // País

        public Address(string street, string number, string complement, string neighborhood, string city, string state, string postalCode, string country)
        {
            
            Street = street;
            Number = number;
            Complement = complement;
            District = neighborhood;
            City = city;
            State = state;
            PostalCode = postalCode;
            Country = country;
        }
        public bool IsValid(Core.Message.INotification notification)
        {
            var validator = new AddressValidation();
            var validationResult = validator.Validate(this);
            validationResult.Errors.ToList().ForEach(x =>
            {
                notification.AddNotifications(x.ErrorMessage);
            });
            return validationResult.IsValid;
        }
        public string GetFullAddress()
        {
            var complementPart = string.IsNullOrWhiteSpace(Complement) ? "" : $", {Complement}";
            return $"{Street}, {Number}{complementPart}, {District}, {City} - {State}, {PostalCode}, {Country}";
        }
    }
}
