namespace Mshop.Core.DomainObject
{
    public abstract class DomainEvent
    {
        public DateTime OccuredOn { get; protected set; }
        public Guid Id { get; protected set; }
        public string Version { get; protected  set; } = "1.0.0";
        public string EventName { get; protected set; }
        public DomainEvent()
        {
            OccuredOn = DateTime.Now;
            EventName = GetType().Name;
        }

    }
}
