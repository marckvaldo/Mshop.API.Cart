
using Mshop.Core.Message;

namespace Mshop.Core.Base
{
    public abstract class BaseService
    {
        protected readonly INotification Notifications;

        protected BaseService(INotification notification)
        {
            Notifications = notification;
        }

        protected void Notificar(string menssagem)
        {
            Notifications.AddNotifications(menssagem);
        }

        protected bool Validate(Notification validation)
        {
            var result = validation.Validate();
            if (result.HasErrors()) return true;

            //Notificar(result.Erros());

            return false;
        }

        protected bool TheareErrors()
        {
            if (Notifications.HasErrors()) return true;
            return false;
        }
    }
}
