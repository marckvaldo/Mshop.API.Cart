using Mshop.Application.Commons.DTO;
using Mshop.Core.DomainObject;
using Mshop.Domain.Entity;

namespace Mshop.Application.Commons.Response
{
    public class CartDTO : IModelOutPut
    {
        public Guid Id { get; set; }

        public List<ProductDTO> Products { get; set; }

        public CustomerDTO Customer { get; set; }
        public CartDTO(Guid id, List<ProductDTO> products, CustomerDTO customer)
        {
            Id = id;
            Products = products;
            Customer = customer;
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

        public static List<ProductDTO> ProductTOProductDTO(List<Product> products)
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
                return new CustomerDTO(Guid.Empty, string.Empty, string.Empty, string.Empty);

            return new CustomerDTO(
                customer.Id, 
                customer.Name, 
                customer.Email, 
                customer.Phone);
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
    }
}
