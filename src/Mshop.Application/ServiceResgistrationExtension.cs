using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mshop.Application.Consumers;
using Mshop.Application.Interface;
using Mshop.Core.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mshop.Application
{
    public static class ServiceResgistrationExtension
    {
        public static void AddAplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<INotification, Notifications>();
            
            services.AddScoped<IProductConsumer, ProductConsumer>();
            services.AddScoped<ICustomerConsumer, CustomerConsumer>();
        }
    }
}
