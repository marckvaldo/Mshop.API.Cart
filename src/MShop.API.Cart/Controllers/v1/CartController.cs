using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
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

        [HttpPost("items")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<bool>> AddItemCart(AddItemToCartCommand request, CancellationToken cancellationToken)
        {
            if (ModelState.IsValid) CustomResponse(ModelState);

            var result = await _mediator.Send(request, cancellationToken);
            return CustomResponse(result);
        }

        [HttpDelete("items")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<bool>> RemoverItem(RemoveItemFromCartCommand request, CancellationToken cancellationToken)
        {
            if (ModelState.IsValid) CustomResponse(ModelState);

            var result = await _mediator.Send(request, cancellationToken);
            return CustomResponse(result);
        }

        [HttpPut("items/quantity")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<bool>> RemoverQuantity(RemoveQuantityFromCommand request, CancellationToken cancellationToken)
        {
            if (ModelState.IsValid) CustomResponse(ModelState);

            var result = await _mediator.Send(request, cancellationToken);
            return CustomResponse(result);
        }

        [HttpPost("customer")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<bool>> AddCustomer(AddCustomerCommand request, CancellationToken cancellationToken)
        {
            if (ModelState.IsValid) CustomResponse(ModelState);

            var result = await _mediator.Send(request, cancellationToken);
            return CustomResponse(result);
        }

        [HttpPost("addresses")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<bool>> AddAddress(AddAddressToCartCommand request, CancellationToken cancellationToken)
        {
            if (ModelState.IsValid) CustomResponse(ModelState);

            var result = await _mediator.Send(request, cancellationToken);
            return CustomResponse(result);
        }

        [HttpPost("payments")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<bool>> AddPayment(AddPaymentCommand request, CancellationToken cancellationToken)
        {
            if (ModelState.IsValid) CustomResponse(ModelState);

            var result = await _mediator.Send(request, cancellationToken);
            return CustomResponse(result);
        }

        [HttpPost("checkout")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<bool>> Checkout(CheckoutCommand request, CancellationToken cancellationToken)
        {
            if (ModelState.IsValid) CustomResponse(ModelState);

            var result = await _mediator.Send(request, cancellationToken);
            return CustomResponse(result);
        }

        [HttpDelete("cart/{cartId:guid}")]
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
