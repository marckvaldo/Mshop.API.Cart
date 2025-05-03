namespace Mshop.Core.Message
{

    public class Notifications : INotification
    {
        private readonly List<MessageError> _erros;

        public Notifications()
        {
            _erros = new List<MessageError>();
        }

        public void AddNotifications(string error)
        {
            _erros.Add(new MessageError(error));
        }

        public void AddNotifications(MessageError error)
        {
            _erros.Add(error);
        }

        public List<MessageError> Errors()
        {
            return _erros;
        }

        public bool HasErrors()
        {
            return _erros.Any();
        }

    }
}
