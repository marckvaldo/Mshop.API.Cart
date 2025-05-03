namespace Mshop.Application.Commons.DTO;

public record CustomerDTO(Guid Id, string Name, string Email, string Phone, AddressDTO Adress = null);
