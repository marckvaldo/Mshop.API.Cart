using Mshop.Application.Commons.DTO;
using Mshop.Core.DomainObject;
using Mshop.Domain.Entity;
using System.Linq;

namespace Mshop.Application.Commons.Response
{
    public class CartResponse : IModelOutPut
    {
        public Guid Id { get; set; }

        public IEnumerable<ProductDTO> Products { get; set; }

        public CustomerDTO Customer { get; set; }

        public IEnumerable<PaymentDTO> Payments { get; set; }

        public CartResponse() { }
        public CartResponse(Guid id, IEnumerable<ProductDTO> products, CustomerDTO customer)
        {
            Id = id;
            Products = products;
            Customer = customer;
        }
        
        public CartResponse(Guid id, IEnumerable<ProductDTO> products, CustomerDTO customer, IEnumerable<PaymentDTO> payments)
        {
            Id = id;
            Products = products;
            Customer = customer;
            Payments = payments;
        }




        public static List<Product> ProductDtoTOProduct(List<ProductDTO> products)
        {
            var produtcsEntity = new List<Product>();
            foreach (var product in products)
            {
                var newProduct = new Product(product.Id,product.Description, product.Name, product.Price,product.IsSale,product.CategoryId,product.Category,product.Thumb, product.Quantity);
                produtcsEntity.Add(newProduct);
            }

            return produtcsEntity;
        }

        public static Customer CustomerDtoTOCustomer(CustomerDTO customer)
        {
            return new Customer(
                customer.Id,
                customer.Name, 
                customer.Email, 
                customer.Phone);
        }

        public static Address AddressDtoTOAddress(AddressDTO address)
        {
            return new Address(
                address.Street, 
                address.Number, 
                address.Complement, 
                address.Neighborhood, 
                address.City, 
                address.State, 
                address.PostalCode, 
                address.Country);
        }

        public static IEnumerable<ProductDTO> ProductTOProductDTO(List<Product> products)
        {
            var produtcsEntity = new List<ProductDTO>();
            foreach (var product in products)
            {
                var newProduct = new ProductDTO(
                    product.Id, 
                    product.Description, 
                    product.Name, 
                    product.Price, 
                    product.Total,
                    product.IsSale, 
                    product.CategoryId, 
                    product.Category,  
                    product.Quantity,
                    product.Thumb);

                produtcsEntity.Add(newProduct);
            }

            return produtcsEntity;
        }

        public static CustomerDTO CustomerTOCustomerDTO(Customer customer)
        {
            if(customer is null)
                return new CustomerDTO(
                    Guid.Empty, 
                    string.Empty, 
                    string.Empty, 
                    string.Empty, 
                    new AddressDTO(
                        string.Empty,
                        string.Empty,
                        string.Empty,
                        string.Empty,
                        string.Empty,
                        string.Empty,
                        string.Empty,
                        string.Empty
                    ));

            return new CustomerDTO(
                customer.Id, 
                customer.Name, 
                customer.Email, 
                customer.Phone,
                new AddressDTO(
                    customer.Address.Street, 
                    customer.Address.Number,
                    customer.Address.Complement,
                    customer.Address.District,
                    customer.Address.City,
                    customer.Address.State,
                    customer.Address.PostalCode,
                    customer.Address.Country)
                    );
        }

        public static AddressDTO AddressTOAddressDTO(Address address)
        {
            return new AddressDTO(
                address.Street, 
                address.Number, 
                address.Complement, 
                address.District, 
                address.City, 
                address.State, 
                address.PostalCode, 
                address.Country);
        }

        public static Payment PaymentDTOTOPayment(PaymentDTO payment)
        {
            if (payment.PaymentMethod is PaymentMethod.CreditCard or PaymentMethod.DebitCard)
            {
                return new Payment(payment.Amount, payment.PaymentMethod, payment.Installments, payment.CardToken);
            }

            if (payment.PaymentMethod is PaymentMethod.BoletoBancario)
            {
                return new Payment(payment.Amount, payment.PaymentMethod, payment.BoletoNumber, payment.BoletoDueDate);
            }

            return new Payment(payment.Amount, payment.PaymentMethod);
        }

        public static IEnumerable<PaymentDTO> PaymentToPaymentDTO(List<Payment> payments)
        {
            var paymentDTOs = new List<PaymentDTO>();
            foreach (var payment in payments)
            {
                var DTO = new PaymentDTO(
                    payment.Amount,
                    payment.PaymentMethod,
                    payment.Status,
                    payment.Installments,
                    payment.CardToken,
                    payment.BoletoNumber,
                    payment.BoletoDueDate);

                paymentDTOs.Add(DTO);
            }

            return paymentDTOs;
        }
    }
}
