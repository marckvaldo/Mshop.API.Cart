using Mshop.Application.Interface;
using Mshop.Core.Message;
using Mshop.Infra.Consumer.DTOs;
using Mshop.Infra.Consumer.GRPC;


namespace Mshop.Application.Consumers
{
    public class CustomerConsumer : ICustomerConsumer
    {

        private readonly IServicerGRPC _servicerGRPC;
        private readonly INotification _notification;
        public CustomerConsumer(IServicerGRPC servicerGRPC, INotification notification)
        {
            _servicerGRPC = servicerGRPC;
            _notification = notification;
        }
        public async Task<CustomerModel?> GetCustomerByIdAsync(Guid customerId)
        {

            if (customerId == Guid.Empty)
            {
                _notification.AddNotifications("CustomerId cannot be empty");
                return null;
            }

            var result = await _servicerGRPC.GetCustomerByIdAsync(customerId);

            if(result == null)
                _notification.AddNotifications("Customer not found");

            return result;
        }
    }
}
