using Microsoft.Extensions.DependencyInjection;
using Mshop.Core.DomainObject;
using Mshop.Core.Message;
using Mshop.Core.Message.DomainEvent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mshop.Application.Event
{
    public class DomainEventPublisher : IDomainEventPublisher
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly INotification _notification;

        public DomainEventPublisher(IServiceProvider serviceProvider, INotification notification)
        {
            _serviceProvider = serviceProvider;
            _notification = notification;
        }

        public async Task<bool> PublishAsync<TDomainEvent>(TDomainEvent Event) where TDomainEvent : DomainEvent
        {
            var handlerType = _serviceProvider.GetServices<IDomainEventHandler<TDomainEvent>>();
            
            if(handlerType is null || !handlerType.Any())
            {
                _notification.AddNotifications($"Handler not found in {Event}");
                return false;
            }

            var result = true;
            foreach (var handler in handlerType)
            {
               
                if(!await handler.HandlerAsync(Event))
                {
                    Console.WriteLine($"Error in handler {handler.GetType().Name} for event {Event}");
                    result = false;
                }
            }
            
            return result;
        }

        public async Task<bool> PublishAsync<TDomainEvent>(IEnumerable<TDomainEvent> events) where TDomainEvent : DomainEvent
        {

            var result = true;

            foreach (var entity in events)
            {
                var handlerType = _serviceProvider.GetServices(typeof(IDomainEventHandler<>).MakeGenericType(entity.GetType()));

                if (handlerType is null || !((IEnumerable<object>)handlerType).Any())
                {
                    Console.WriteLine($"Handler not found for {entity.GetType().Name}");
                    result = false;
                    continue;
                }

                foreach (var handler in (IEnumerable<object>)handlerType)
                {
                    var method = handler.GetType().GetMethod("HandlerAsync");
                    if (method != null)
                    {
                        var handlerResult = (Task<bool>)method.Invoke(handler, new object[] { entity });
                        if (!await handlerResult)
                        {
                            Console.WriteLine($"Error in handler {handler.GetType().Name} for event {entity}");
                            result = false;
                        }
                    }
                }
            }

            return result;
            
        }
    }
}
