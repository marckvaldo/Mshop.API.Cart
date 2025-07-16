namespace Mshop.E2ETests.GraphQL.Common
{
    public record CustomerPayload(Guid Id, string Name, string Email, string Phone, AddressPayload Adress = null);

}
