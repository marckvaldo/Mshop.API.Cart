using Grpc.Net.Client;
using Microsoft.Extensions.Options;
using Mshop.Infra.Consumer.CircuitBreakerPolicy;
using Mshop.Infra.Consumer.DTOs;
using Mshop.Infra.Consumer.Protos;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Mshop.Infra.Consumer.GRPC
{
    public class ServiceGRPC : IServicerGRPC
    {
        private readonly SettingsGRPC _options;
        private readonly string _grpcEndpointProduct;
        private readonly string _grpcEndpointCustomer;
        private readonly ICircuitBreaker _circuitBreaker;

        public ServiceGRPC(IOptions<SettingsGRPC> options, ICircuitBreaker circuitBreaker)
        {
            _options = options.Value;
            _grpcEndpointProduct = _options.GrpcProduct;
            _grpcEndpointCustomer = _options.GrpcCustomer;
            _circuitBreaker = circuitBreaker;

            circuitBreaker.Start(
                ex =>
                {
                    return ex is Grpc.Core.RpcException || ex is TimeoutException || ex is Exception;
                },
                1,
                TimeSpan.FromMinutes(1)
                );
        }

        public Task<CustomerModel?> GetCustomerByIdAsync(Guid customerId)
        {
            throw new NotImplementedException();
        }
        public async Task<ProductModel?> GetProductByIdAsync(Guid productId)
        {
            //try
            //{
            // Create a channel to the gRPC endpoint
            /*using var channel = GrpcChannel.ForAddress(_grpcEndpointProduct);

            // Create a gRPC client
            var client = new ProductProto.ProductProtoClient(channel);

            // Prepare the request
            var request = new GetProductRequest { Id = productId.ToString() };

            // Call the gRPC service
            var response = await client.GetProductByIdAsync(request);

            // Handle the response
            if (response.Success)
            {
                return new ProductModel(Guid.Parse(response.Data.Id),
                    response.Data.Description,
                    response.Data.Name,
                    ((decimal)response.Data.Price),
                    response.Data.IsPromotion,
                    Guid.Parse(response.Data.CategoryId),
                    response.Data.Category,
                    response.Data.Thumb);
            }

            // Log or handle errors if needed
            // For now, just returning null on failure
            return null;*/
            /*}
            catch (Exception ex)
            {
                // Log the exception (use a logging framework here)
                Console.WriteLine($"Error calling gRPC service: {ex.Message}");
                return null;
            }*/
            
            try
            {
                return await _circuitBreaker.ExecuteActionAsync(async () => await GetProductByIdGRPC(productId));
            }
            catch(Grpc.Core.RpcException error)
            {
                Console.WriteLine($"CircuitBreaker ativado devido a: {error.Message}");
                return null;    
            }
            catch(Exception error)
            {
                Console.WriteLine($"CircuitBreaker ativado devido a: {error.Message}");
                return null;
            }

            //return await GetProductByIdGRPC(productId);
        }

        private async Task<ProductModel?> GetProductByIdGRPC(Guid productId)
        {
            using (var channel = GrpcChannel.ForAddress(_grpcEndpointProduct))
            {
                var client = new ProductProto.ProductProtoClient(channel);

                var request = new GetProductRequest { Id = productId.ToString() };
                var response = await client.GetProductByIdAsync(request);

                if (response.Success)
                {
                    return new ProductModel(
                        Guid.Parse(response.Data.Id),
                        response.Data.Description,
                        response.Data.Name,
                        (decimal)response.Data.Price,
                        response.Data.IsPromotion,
                        Guid.Parse(response.Data.CategoryId),
                        response.Data.Category,
                        response.Data.Thumb);
                }

                return null;
            }
        }
    }
}
