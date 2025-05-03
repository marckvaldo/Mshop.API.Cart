using CoreObject = Mshop.Core.DomainObject;

namespace Mshop.Core.Message.DomainEvent
{
    public interface IDomainEventHandler<TDomainEvent> where TDomainEvent : CoreObject.DomainEvent
    {
        Task<bool> HandlerAsync(TDomainEvent domainEvent);
    }
}
