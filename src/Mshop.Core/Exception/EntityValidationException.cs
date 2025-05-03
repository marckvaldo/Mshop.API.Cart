using Mshop.Core.Message;

namespace Mshop.Core.Exception
{
    public class EntityValidationException : ApplicationException
    {
        private readonly INotification _notification;
        public EntityValidationException(
            string message
            ) : base(message)
        {
        }
    }


}