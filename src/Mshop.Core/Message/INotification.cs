namespace Mshop.Core.Message
{
    public interface INotification
    {
        bool HasErrors();

        List<MessageError> Errors();

        void AddNotifications(string error);


    }
}
