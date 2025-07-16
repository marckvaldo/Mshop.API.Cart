using Mshop.Core.Message;

namespace Mshop.API.GraphQL.Cart.GraphQL
{
    public abstract class MainGraphQL
    {
        public void Notify(string message, INotification notification)
        {
            notification.AddNotifications(message);
        }

        public void NotifyWithException(string message, INotification notifications)
        {
            notifications.AddNotifications(message);
            throw new GraphQLException(ConvertToGraphQLErrors(notifications));
        }

        public void RequestIsValid(INotification notifications)
        {
            if (notifications.HasErrors())
            {
                throw new GraphQLException(ConvertToGraphQLErrors(notifications));
            }
        }

        private IError[] ConvertToGraphQLErrors(INotification notifications)
        {
            return notifications.Errors().Select(error =>
                ErrorBuilder.New()
                    .SetMessage(error.Message)
                    .SetCode("CUSTOM_ERROR_CODE") // Opcionalmente, defina um código de erro
                    .Build()
            ).ToArray();
        }
    }
}
