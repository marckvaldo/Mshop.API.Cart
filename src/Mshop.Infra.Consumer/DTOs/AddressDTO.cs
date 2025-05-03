namespace Mshop.Infra.Consumer.DTOs;

public record AddressDTO(string Street, string Number, string Complement, string Neighborhood, string City, string State, string PostalCode, string Country);
