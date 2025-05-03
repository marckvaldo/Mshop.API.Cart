using Mshop.Application.Interface;
using Mshop.Core.Message;
using Mshop.Infra.Consumer.Cache;
using Mshop.Infra.Consumer.DTOs;
using Mshop.Infra.Consumer.GRPC;


namespace Mshop.Application.Consumers
{
    public class ProductConsumer : IProductConsumer
    {
        private readonly IServiceCache _serviceCache;
        private readonly IServicerGRPC _servicerGRPC;
        private readonly INotification _notification;

        public ProductConsumer(IServiceCache serviceCache, IServicerGRPC servicerGRPC, INotification notification)
        {
            _serviceCache = serviceCache;
            _servicerGRPC = servicerGRPC;
            _notification = notification;
        }

        public async Task<ProductModel?> GetProductByIdAsync(Guid productId)
        {
            if (productId == Guid.Empty)
            {
                _notification.AddNotifications("ProductId cannot be empty");
                return null;
            }
            
            var result = await _servicerGRPC.GetProductByIdAsync(productId) ?? await _serviceCache.GetProductById(productId);
            
            if (result == null)
                _notification.AddNotifications("Product not found");

            return result;

        }
    }
}
