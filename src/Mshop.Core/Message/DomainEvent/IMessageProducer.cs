namespace Mshop.Core.Message.DomainEvent
{
    public interface IMessageProducer
    {
        Task SendMessageAsync<T>(T message);
    }
}
