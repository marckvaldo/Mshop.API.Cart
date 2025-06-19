using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mshop.IntegrationTest.Common.Persistence.RabbitMQ.DTOs
{
    public class AddressDTO
    {
        public string Street { get;  set; } // Rua
        public string Number { get;  set; } // Número
        public string Complement { get;  set; } // Complemento (opcional)
        public string District { get;  set; } // Bairro
        public string City { get;  set; } // Cidade
        public string State { get;  set; } // Estado
        public string PostalCode { get;  set; } // CEP
        public string Country { get;  set; } // País

    }
}
