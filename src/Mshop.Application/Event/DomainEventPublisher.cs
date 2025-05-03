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
                    _notification.AddNotifications($"Error in handler {handler.GetType().Name} for event {Event}");
                    result = false;
                }
            }
            
            return result;
        }
    }
}
