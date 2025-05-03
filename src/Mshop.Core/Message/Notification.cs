namespace Mshop.Core.Message
{
    public abstract class Notification
    {
        protected readonly INotification _notifications;

        protected Notification(INotification notifications)
        {
            _notifications = notifications;
        }
        public abstract INotification Validate();
    }
}
