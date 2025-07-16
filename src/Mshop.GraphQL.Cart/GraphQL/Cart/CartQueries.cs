using MediatR;
using Mshop.Application.Services.Cart.Queries;
using Message = Mshop.Core.Message;
using Mshop.API.GraphQL.Cart.GraphQL.Cart.DTO;

namespace Mshop.API.GraphQL.Cart.GraphQL.Cart
{
    [ExtendObjectType(OperationTypeNames.Query)]
    public class CartQueries : MainGraphQL
    {
        public async Task<CartPayload> GetCartByIdAsync(
            [Service] IMediator mediator,
            [Service] Message.INotification notification,
            Guid id,
            CancellationToken cancellationToken)
        {
            var request = new GetCartDetailsQuery(id);
            var outPut = await mediator.Send(request, cancellationToken);

            RequestIsValid(notification);

            return new CartPayload(
                id,
                (outPut.Data.Products).Select(
                    x => new ProductPayload(x.Id,
                    x.Description,
                    x.Name, x.Price,
                    x.Total, x.IsSale,
                    x.CategoryId,
                    x.Category,
                    x.Quantity,
                    x.Thumb)),
                new CustomerPayload(
                    outPut.Data.Customer.Id,
                    outPut.Data.Customer.Name,
                    outPut.Data.Customer.Email,
                    outPut.Data.Customer.Phone,
                        new AddressPayload(
                            outPut.Data.Customer.Adress.Street, 
                            outPut.Data.Customer.Adress.Number, 
                            outPut.Data.Customer.Adress.Complement,
                            outPut.Data.Customer.Adress.Neighborhood,
                            outPut.Data.Customer.Adress.City,
                            outPut.Data.Customer.Adress.State,
                            outPut.Data.Customer.Adress.PostalCode,
                            outPut.Data.Customer.Adress.Country)),
                outPut.Data.Payments.Select(p => 
                new PaymentPayload(p.Amount, 
                    p.PaymentMethod.ToString(), 
                    p.PaymentStatus.ToString(), 
                    p.Installments, 
                    p.CardToken, 
                    p.BoletoNumber, 
                    p.BoletoDueDate)));

        }

        public async Task<CartPayload> GetCartByCustomerIdAsync(
            [Service] IMediator mediator,
            [Service] Message.INotification notification,
            Guid customerId,
            CancellationToken cancellationToken)
        {
            var request = new GetCartByCustomerIdQuery(customerId);
            var outPut = await mediator.Send(request, cancellationToken);

            RequestIsValid(notification);

            return new CartPayload(
                outPut.Data.Id,
                outPut.Data.Products.Select(x => 
                    new ProductPayload(
                        x.Id, 
                        x.Description, 
                        x.Name, 
                        x.Price, 
                        x.Total, 
                        x.IsSale, 
                        x.CategoryId, 
                        x.Category, 
                        x.Quantity, 
                        x.Thumb)),
                new CustomerPayload(
                    outPut.Data.Customer.Id, 
                    outPut.Data.Customer.Name, 
                    outPut.Data.Customer.Email, 
                    outPut.Data.Customer.Phone,
                        new AddressPayload(
                            outPut.Data.Customer.Adress.Street,
                            outPut.Data.Customer.Adress.Number,
                            outPut.Data.Customer.Adress.Complement,
                            outPut.Data.Customer.Adress.Neighborhood,
                            outPut.Data.Customer.Adress.City,
                            outPut.Data.Customer.Adress.State,
                            outPut.Data.Customer.Adress.PostalCode,
                            outPut.Data.Customer.Adress.Country)),
                outPut.Data.Payments.Select(p => 
                new PaymentPayload(
                    p.Amount, 
                    p.PaymentMethod.ToString(), 
                    p.PaymentStatus.ToString(), 
                    p.Installments, 
                    p.CardToken, 
                    p.BoletoNumber, 
                    p.BoletoDueDate)));
        }
    }
}
