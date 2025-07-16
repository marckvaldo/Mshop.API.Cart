using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Mshop.API.Cart.Requests.V1.Cart;
using Mshop.Application.Commons.DTO;
using Mshop.Application.Commons.Response;
using Mshop.Application.Services.Cart.Commands;
using Mshop.Application.Services.Cart.Queries;
using MessageCore = Mshop.Core.Message;

namespace MShop.API.Cart.Controllers.v1
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion(1.0)]

    public class CartController : MainController
    {
        private IMediator _mediator;
        public CartController(MessageCore.INotification notification, IMediator mediator) : base(notification)
        {
            _mediator = mediator;
        }

        [HttpGet("{cartId:guid}")]
        [ProducesResponseType(typeof(CartResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CartResponse>> GetCart(Guid cartId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetCartDetailsQuery(cartId));
            if (result.Data is null) return CustomResponse(404);
            return CustomResponse(result);
        }

        [HttpGet("customer/{customerId:guid}")]
        [ProducesResponseType(typeof(CartResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CartResponse>> GetCartByCustomerId(Guid customerId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetCartByCustomerIdQuery(customerId));
            if (result.Data is null) return CustomResponse(404);
            return CustomResponse(result);
        }

        [HttpPost("{cartId}/items")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<bool>> AddItemCart(Guid cartId,[FromBody] AddItemToCartRequest request, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);
                
            var command = new AddItemToCartCommand(cartId, request.ProductId, request.Quantity);
            var result = await _mediator.Send(command, cancellationToken);
            return CustomResponse(result);
        }

        [HttpDelete("{cartId}/items/{productId}")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<bool>> RemoverItem(Guid cartId, Guid productId, CancellationToken cancellationToken)
        {
            RemoveItemFromCartCommand request = new RemoveItemFromCartCommand(cartId,productId);
            var result = await _mediator.Send(request, cancellationToken);
            return CustomResponse(result);
        }

        [HttpPut("{cartId}/items/{productId}/LessQuantity")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<bool>> LessQuantity(Guid cartId, Guid productId, [FromBody] LessQuantityItemRequest request, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            var command = new RemoveQuantityFromCommand(cartId, productId, request.Quantity);
            var result = await _mediator.Send(command, cancellationToken);
            return CustomResponse(result);
        }

        [HttpPost("{cartId}/customer")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<bool>> AddCustomer(Guid cartId, [FromBody] AddCustomerToCartRequest request, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);
            var command = new AddCustomerToCartCommand(cartId, request.CustomerId);
            var result = await _mediator.Send(command, cancellationToken);
            return CustomResponse(result);
        }

        [HttpPost("{cartId}/addresses")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<bool>> AddAddress(Guid cartId, [FromBody] AddAddressToCartRequest request, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            var command = new AddAddressToCartCommand(cartId, 
                    new AddressDTO(
                        request.Street, 
                        request.Number,
                        request.Complement,
                        request.Neighborhood,
                        request.City,request.State, 
                        request.PostalCode,
                        request.Country));

            var result = await _mediator.Send(command, cancellationToken);
            return CustomResponse(result);
        }

        [HttpPost("{cartId}/payments")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<bool>> AddPayment(Guid cartId, [FromBody] AddPaymentToCartRequest request, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            var command = new AddPaymentToCartCommand(cartId,
                new PaymentDTO(
                    request.Amount,
                    request.PaymentMethod,
                    Mshop.Domain.Entity.PaymentStatus.Pending,
                    request.Installments,
                    request.CardToken,
                    request.BoletoNumber,
                    request.BoletoDueDate
                    ));

            var result = await _mediator.Send(command, cancellationToken);
            return CustomResponse(result);
        }

        [HttpGet("{cartId}/checkout")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<bool>> Checkout(Guid cartId, CancellationToken cancellationToken)
        {
            if (ModelState.IsValid) CustomResponse(ModelState);
            var command = new CheckoutCommand(cartId);
            var result = await _mediator.Send(command, cancellationToken);
            return CustomResponse(result);
        }

        [HttpDelete("{cartId:guid}/items")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<bool>> ClearCart(Guid cartId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new ClearCartCommand(cartId), cancellationToken);
            return CustomResponse(result);
        }

        [HttpGet("ping")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public ActionResult<string> Ping()
        {
            return CustomResponse("Pong", StatusCodes.Status200OK);
        }
    }
}
