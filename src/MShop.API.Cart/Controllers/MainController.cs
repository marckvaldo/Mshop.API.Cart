using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Mshop.Core.DomainObject;
using Mshop.Core.Message;
using MShop.API.Cart.Extension;

namespace MShop.API.Cart.Controllers
{
    [ApiController]
    abstract public class MainController : ControllerBase
    {
        private readonly INotification _notification;

        protected MainController(INotification notification)
        {
            _notification = notification;
        }

        protected bool OperationIsValid()
        {
            return !_notification.HasErrors();
        }

        protected ActionResult CustomResponse(ModelStateDictionary modelState)
        {
            if (!modelState.IsValid) NotifyInvalidModelError(modelState);

            return CustomResponse();
        }

        protected ActionResult CustomResponse<T>(Result<T> result, int statusCode = StatusCodes.Status200OK) where T : IModelOutPut
        {
            return CustomResponse(result.Data, statusCode);
        }

        protected ActionResult CustomResponse(int statusCode)
        {
            return CustomResponse(null, statusCode);
        }

        protected ActionResult CustomResponse(object result = null, int statusCode = StatusCodes.Status200OK)
        {
            if (OperationIsValid())
            {
                if (result is null)
                    return NoContent();

                return StatusCode(statusCode, ExtensionResponse.Success(result));
            }

            var erros = ExtensionResponse.Error(_notification.Errors().Select(x => x.Message).ToList());

            return statusCode == StatusCodes.Status404NotFound ? NotFound(erros) : BadRequest(erros);
        }

        protected void NotifyInvalidModelError(ModelStateDictionary modelState)
        {
            var errors = modelState.Values.SelectMany(e => e.Errors);

            foreach (var error in errors)
            {
                var errorMsg = error.Exception == null ? error.ErrorMessage : error.Exception.Message;
                Notify(errorMsg);
            }
        }

        protected void Notify(string messagem)
        {
            if (messagem.Length > 0)
                _notification.AddNotifications(messagem);
        }
    }
}
