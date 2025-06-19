using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mshop.Application.Consumers;
using Mshop.Application.Event;
using Mshop.Application.Event.Order;
using Mshop.Application.Interface;
using Mshop.Core.Message;
using Mshop.Core.Message.DomainEvent;
using Mshop.Domain.Event;


namespace Mshop.Application
{
    public static class ServiceResgistrationExtension
    {
        public static IServiceCollection AddAplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<INotification, Notifications>();
            
            services.AddScoped<IProductConsumer, ProductConsumer>();
            services.AddScoped<ICustomerConsumer, CustomerConsumer>();

            //eventos
            services.AddScoped<IDomainEventPublisher,DomainEventPublisher>();
            services.AddScoped<IDomainEventHandler<OrderCheckoutedEvent>, OrderCheckoutedEventHandler>();

            services.AddMediatR(x => x.RegisterServicesFromAssemblies(typeof(ServiceResgistrationExtension).Assembly));

            return services;
        }
    }
}
