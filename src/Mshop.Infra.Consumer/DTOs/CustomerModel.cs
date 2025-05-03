namespace Mshop.Infra.Consumer.DTOs
{
    public record CustomerModel(Guid Id, string Name, string Email, string Phone, AddressDTO Adress = null);

}
