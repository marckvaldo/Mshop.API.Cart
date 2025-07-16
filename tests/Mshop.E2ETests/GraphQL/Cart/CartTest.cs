using Microsoft.Extensions.DependencyInjection;
using Mshop.E2ETests.GraphQL.Common;
using Mshop.Infra.Data.Interface;
using NetTopologySuite.Index.HPRtree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mshop.E2ETests.GraphQL.Cart
{
    public class CartTest : CartFixtureTest
    {
        protected ICartRepository _cartRepository;
        public CartTest() : base()
        {
            _cartRepository = _serviceProvider.GetRequiredService<ICartRepository>();
        }

        [Fact(DisplayName = "Should Show a Shopping Cart by Cart id")]
        [Trait("End2End - GraphQL", "Shopping")]
        public async void ShouldShowShoppingCartByCartId()
        {
            var cart = FakerCart(true,true,true);
            //2381068e-8b4d-46b2-9230-6680b8e43739

            await _cartRepository.AddAsync(cart, CancellationToken.None);

            string query = $@"
                        {{
                            cartById(id:""{cart.Id}"")
                            {{
                                id,
                                customer{{
                                  id,
                                  name,
                                  email,
                                  phone,
                                  adress{{
                                    number,
                                    complement,
                                    neighborhood,
                                    city,
                                    state,
                                    postalCode,
                                    country           
                                  }}
                                }},
                                products{{
                                  id,
                                  description,
                                  name,
                                  price,
                                  total,
                                  isSale,
                                  categoryId,
                                  category,
                                  quantity,
                                  thumb
                                }},
                                payments{{
                                  amount,
                                  paymentMethod,
                                  paymentStatus,
                                  installments,
                                  cardToken,
                                  boletoNumber,
                                  boletoDueDate
                                }}  
                            }}
                        }}";


            var result = await _graphQlClient.SendQueryAsync<CustomResponse<RootResponseByCartId>>(query);

            Assert.NotNull(result);
            Assert.Equal(result.Data.CartById.Id, cart.Id);
            Assert.Equal(result.Data.CartById.Customer.Name, cart.Customer.Name);
            Assert.Equal(result.Data.CartById.Customer.Email, cart.Customer.Email);
            Assert.Equal(result.Data.CartById.Customer.Phone, cart.Customer.Phone);
            Assert.NotEmpty(result.Data.CartById.Products);
            var produtos = result.Data.CartById.Products.ToList();
            foreach(var item in produtos)
            {
                Assert.Equal(item.Name, cart.Products.FirstOrDefault(x => x.Id == item.Id)?.Name);
                Assert.Equal(item.Description, cart.Products.FirstOrDefault(x => x.Id == item.Id)?.Description);
                Assert.Equal(item.Price, cart.Products.FirstOrDefault(x => x.Id == item.Id)?.Price);
                Assert.Equal(item.Total, cart.Products.FirstOrDefault(x => x.Id == item.Id)?.Total);
                Assert.Equal(item.IsSale, cart.Products.FirstOrDefault(x => x.Id == item.Id)?.IsSale);
                Assert.Equal(item.CategoryId, cart.Products.FirstOrDefault(x => x.Id == item.Id)?.CategoryId);
                Assert.Equal(item.Category, cart.Products.FirstOrDefault(x => x.Id == item.Id)?.Category);
                Assert.Equal(item.Quantity, cart.Products.FirstOrDefault(x => x.Id == item.Id)?.Quantity);
                Assert.Equal(item.Thumb, cart.Products.FirstOrDefault(x => x.Id == item.Id)?.Thumb);
            }

            var payments = result.Data.CartById.Payments.ToList();
            foreach(var item in payments)
            {
                Assert.Equal(item.Amount, cart.Payments.FirstOrDefault(x => x.PaymentMethod.ToString() == item.PaymentMethod)?.Amount);
                Assert.Equal(item.PaymentMethod, cart.Payments.FirstOrDefault(x => x.PaymentMethod.ToString() == item.PaymentMethod)?.PaymentMethod.ToString());
                Assert.Equal(item.PaymentStatus, cart.Payments.FirstOrDefault(x => x.PaymentMethod.ToString() == item.PaymentMethod)?.Status.ToString());
                Assert.Equal(item.Installments, cart.Payments.FirstOrDefault(x => x.PaymentMethod.ToString() == item.PaymentMethod)?.Installments);
                Assert.Equal(item.CardToken, cart.Payments.FirstOrDefault(x => x.PaymentMethod.ToString() == item.PaymentMethod)?.CardToken);
                Assert.Equal(item.BoletoNumber, cart.Payments.FirstOrDefault(x => x.PaymentMethod.ToString() == item.PaymentMethod)?.BoletoNumber);
                Assert.Equal(item.BoletoDueDate, cart.Payments.FirstOrDefault(x => x.PaymentMethod.ToString() == item.PaymentMethod)?.BoletoDueDate);
            }            
        }

   



        [Fact(DisplayName = "Should Show a Shopping Cart by Customer Id")]
        [Trait("End2End - GraphQL", "Shopping")]
        public async void ShouldShowShoppingCartByCustomerId()
        {
            var cart = FakerCart(true, true, true);

            await _cartRepository.AddAsync(cart, CancellationToken.None);

            string query = $@"
                        {{
                            cartByCustomerId(customerId:""{cart.Customer.Id}"")
                            {{
                                id,
                                customer{{
                                  id,
                                  name,
                                  email,
                                  phone,
                                  adress{{
                                    number,
                                    complement,
                                    neighborhood,
                                    city,
                                    state,
                                    postalCode,
                                    country           
                                  }}
                                }},
                                products{{
                                  id,
                                  description,
                                  name,
                                  price,
                                  total,
                                  isSale,
                                  categoryId,
                                  category,
                                  quantity,
                                  thumb
                                }},
                                payments{{
                                  amount,
                                  paymentMethod,
                                  paymentStatus,
                                  installments,
                                  cardToken,
                                  boletoNumber,
                                  boletoDueDate
                                }} 
                            }}
                        }}";


            var result = await _graphQlClient.SendQueryAsync<CustomResponse<RootResponseByCustomerId>>(query);

            Assert.NotNull(result);
            /*Assert.Equal(result.Data.CartById.Id, cart.Id);
            Assert.Equal(result.Data.CartById.Customer.Name, cart.Customer.Name);
            Assert.Equal(result.Data.CartById.Customer.Email, cart.Customer.Email);
            Assert.Equal(result.Data.CartById.Customer.Phone, cart.Customer.Phone);
            Assert.NotEmpty(result.Data.CartById.Products);
            var produtos = result.Data.CartById.Products.ToList();
            foreach (var item in produtos)
            {
                Assert.Equal(item.Name, cart.Products.FirstOrDefault(x => x.Id == item.Id)?.Name);
                Assert.Equal(item.Description, cart.Products.FirstOrDefault(x => x.Id == item.Id)?.Description);
                Assert.Equal(item.Price, cart.Products.FirstOrDefault(x => x.Id == item.Id)?.Price);
                Assert.Equal(item.Total, cart.Products.FirstOrDefault(x => x.Id == item.Id)?.Total);
                Assert.Equal(item.IsSale, cart.Products.FirstOrDefault(x => x.Id == item.Id)?.IsSale);
                Assert.Equal(item.CategoryId, cart.Products.FirstOrDefault(x => x.Id == item.Id)?.CategoryId);
                Assert.Equal(item.Category, cart.Products.FirstOrDefault(x => x.Id == item.Id)?.Category);
                Assert.Equal(item.Quantity, cart.Products.FirstOrDefault(x => x.Id == item.Id)?.Quantity);
                Assert.Equal(item.Thumb, cart.Products.FirstOrDefault(x => x.Id == item.Id)?.Thumb);
            }

            var payments = result.Data.CartById.Payments.ToList();
            foreach (var item in payments)
            {
                Assert.Equal(item.Amount, cart.Payments.FirstOrDefault(x => x.PaymentMethod.ToString() == item.PaymentMethod)?.Amount);
                Assert.Equal(item.PaymentMethod, cart.Payments.FirstOrDefault(x => x.PaymentMethod.ToString() == item.PaymentMethod)?.PaymentMethod.ToString());
                Assert.Equal(item.PaymentStatus, cart.Payments.FirstOrDefault(x => x.PaymentMethod.ToString() == item.PaymentMethod)?.Status.ToString());
                Assert.Equal(item.Installments, cart.Payments.FirstOrDefault(x => x.PaymentMethod.ToString() == item.PaymentMethod)?.Installments);
                Assert.Equal(item.CardToken, cart.Payments.FirstOrDefault(x => x.PaymentMethod.ToString() == item.PaymentMethod)?.CardToken);
                Assert.Equal(item.BoletoNumber, cart.Payments.FirstOrDefault(x => x.PaymentMethod.ToString() == item.PaymentMethod)?.BoletoNumber);
                Assert.Equal(item.BoletoDueDate, cart.Payments.FirstOrDefault(x => x.PaymentMethod.ToString() == item.PaymentMethod)?.BoletoDueDate);
            }*/
        }

    }
}
