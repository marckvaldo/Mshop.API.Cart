using Microsoft.Extensions.DependencyInjection;
using Mshop.Infra.Data.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mshop.IntegrationTest.Repository.Data.Repository
{
    [Collection("Repository Cart Collection")]
    [CollectionDefinition("Repository Cart Collection", DisableParallelization = true)]
    public class CartRepositoryTest : CartRepositoryTestFixture
    {
        private readonly ICartRepository _repositoryProduct;
        public CartRepositoryTest() : base()
        {
            _repositoryProduct = _serviceProvider.GetRequiredService<ICartRepository>();
            DropCollection(_serviceProvider, "Carts").Wait();
        }

        [Fact(DisplayName = nameof(CreateCart))]
        [Trait("Integration - Infra.Data", "Cart Repositorio")]

        public async Task CreateCart()
        {
            var cart = FakerCart(true, true);

            await _repositoryProduct.AddAsync(cart, CancellationToken.None);
            var cartDb = await _repositoryProduct.GetByIdAsync(cart.Id);

            Assert.NotNull(cartDb);
            Assert.Equal(cart.Id, cartDb.Id);
            Assert.Equal(cart.Customer.Id, cartDb.Customer.Id);
            Assert.Equal(cart.Customer.Name, cartDb.Customer.Name);
            Assert.Equal(cart.Customer.Email, cartDb.Customer.Email);
            Assert.Equal(cart.Customer.Phone, cartDb.Customer.Phone);
            Assert.Equal(cart.Customer.Address.Country, cartDb.Customer.Address.Country);
            Assert.Equal(cart.Customer.Address.State, cartDb.Customer.Address.State);
            Assert.Equal(cart.Customer.Address.City, cartDb.Customer.Address.City);
            Assert.Equal(cart.Customer.Address.Street, cartDb.Customer.Address.Street);
            Assert.Equal(cart.Customer.Address.PostalCode, cartDb.Customer.Address.PostalCode);
            Assert.Equal(cart.Customer.Address.District, cartDb.Customer.Address.District);
            Assert.Equal(cart.Customer.Address.Number, cartDb.Customer.Address.Number);
            Assert.Equal(cart.Customer.Address.Complement, cartDb.Customer.Address.Complement);
            Assert.Equal(cart.Products.Count(), cartDb.Products.Count());
            foreach (var product in cartDb.Products)
            {
                Assert.Equal(cart.Products.FirstOrDefault(p => p.Id == product.Id).Id, product.Id);
                Assert.Equal(cart.Products.FirstOrDefault(p => p.Id == product.Id).Name, product.Name);
                Assert.Equal(cart.Products.FirstOrDefault(p => p.Id == product.Id).Price, product.Price);
                Assert.Equal(cart.Products.FirstOrDefault(p => p.Id == product.Id).Quantity, product.Quantity);
            }
        }


        [Fact(DisplayName = nameof(UpdateCart))]
        [Trait("Integration - Infra.Data", "Cart Repositorio")]

        public async Task UpdateCart()
        {
            var cart = FakerCart(true, true);

            await _repositoryProduct.AddAsync(cart, CancellationToken.None);

            cart.UpdateCustomer(FakerCustomerWithAddress());
            cart.RemoveItem(cart.Products.FirstOrDefault().Id);

            await _repositoryProduct.UpdateAsync(cart, CancellationToken.None);

            var cartDb = await _repositoryProduct.GetByIdAsync(cart.Id);

            Assert.NotNull(cartDb);
            Assert.Equal(cart.Id, cartDb.Id);
            Assert.Equal(cart.Customer.Id, cartDb.Customer.Id);
            Assert.Equal(cart.Customer.Name, cartDb.Customer.Name);
            Assert.Equal(cart.Customer.Email, cartDb.Customer.Email);
            Assert.Equal(cart.Customer.Phone, cartDb.Customer.Phone);
            Assert.Equal(cart.Customer.Address.Country, cartDb.Customer.Address.Country);
            Assert.Equal(cart.Customer.Address.State, cartDb.Customer.Address.State);
            Assert.Equal(cart.Customer.Address.City, cartDb.Customer.Address.City);
            Assert.Equal(cart.Customer.Address.Street, cartDb.Customer.Address.Street);
            Assert.Equal(cart.Customer.Address.PostalCode, cartDb.Customer.Address.PostalCode);
            Assert.Equal(cart.Customer.Address.District, cartDb.Customer.Address.District);
            Assert.Equal(cart.Customer.Address.Number, cartDb.Customer.Address.Number);
            Assert.Equal(cart.Customer.Address.Complement, cartDb.Customer.Address.Complement);
            Assert.Equal(cart.Products.Count(), cartDb.Products.Count());

            foreach (var product in cartDb.Products)
            {
                Assert.Equal(cart.Products.FirstOrDefault(p => p.Id == product.Id).Id, product.Id);
                Assert.Equal(cart.Products.FirstOrDefault(p => p.Id == product.Id).Name, product.Name);
                Assert.Equal(cart.Products.FirstOrDefault(p => p.Id == product.Id).Price, product.Price);
                Assert.Equal(cart.Products.FirstOrDefault(p => p.Id == product.Id).Quantity, product.Quantity);
            }
        }


        [Fact(DisplayName = nameof(DeleteCart))]
        [Trait("Integration - Infra.Data", "Cart Repositorio")]

        public async Task DeleteCart()
        {
            var cart = FakerCart(true, true);

            await _repositoryProduct.AddAsync(cart, CancellationToken.None);
            await _repositoryProduct.DeleteAsync(cart.Id, CancellationToken.None);
            var cartDb = await _repositoryProduct.GetByIdAsync(cart.Id);

            Assert.Null(cartDb);
        }


        [Fact(DisplayName = nameof(GetCartById))]
        [Trait("Integration - Infra.Data", "Cart Repositorio")]

        public async Task GetCartById()
        {
            var cart = FakerCart(true, true);

            await _repositoryProduct.AddAsync(cart, CancellationToken.None);
            var cartDb = await _repositoryProduct.GetByIdAsync(cart.Id);

            Assert.NotNull(cartDb);
            Assert.Equal(cart.Id, cartDb.Id);
            Assert.Equal(cart.Customer.Id, cartDb.Customer.Id);
            Assert.Equal(cart.Customer.Name, cartDb.Customer.Name);
            Assert.Equal(cart.Customer.Email, cartDb.Customer.Email);
            Assert.Equal(cart.Customer.Phone, cartDb.Customer.Phone);
            Assert.Equal(cart.Customer.Address.Country, cartDb.Customer.Address.Country);
            Assert.Equal(cart.Customer.Address.State, cartDb.Customer.Address.State);
            Assert.Equal(cart.Customer.Address.City, cartDb.Customer.Address.City);
            Assert.Equal(cart.Customer.Address.Street, cartDb.Customer.Address.Street);
            Assert.Equal(cart.Customer.Address.PostalCode, cartDb.Customer.Address.PostalCode);
            Assert.Equal(cart.Customer.Address.District, cartDb.Customer.Address.District);
            Assert.Equal(cart.Customer.Address.Number, cartDb.Customer.Address.Number);
            Assert.Equal(cart.Customer.Address.Complement, cartDb.Customer.Address.Complement);
            Assert.Equal(cart.Products.Count(), cartDb.Products.Count());
            foreach (var product in cartDb.Products)
            {
                Assert.Equal(cart.Products.FirstOrDefault(p => p.Id == product.Id).Id, product.Id);
                Assert.Equal(cart.Products.FirstOrDefault(p => p.Id == product.Id).Name, product.Name);
                Assert.Equal(cart.Products.FirstOrDefault(p => p.Id == product.Id).Price, product.Price);
                Assert.Equal(cart.Products.FirstOrDefault(p => p.Id == product.Id).Quantity, product.Quantity);
            }
        }

        [Fact(DisplayName = nameof(GetCartByCustomerId))]
        [Trait("Integration - Infra.Data", "Cart Repositorio")]

        public async Task GetCartByCustomerId()
        {
            var cart = FakerCart(true, true);

            await _repositoryProduct.AddAsync(cart, CancellationToken.None);
            var cartDb = await _repositoryProduct.GetByCustomerId(cart.Customer.Id);

            Assert.NotNull(cartDb);
            Assert.Equal(cart.Id, cartDb.Id);
            Assert.Equal(cart.Customer.Id, cartDb.Customer.Id);
            Assert.Equal(cart.Customer.Name, cartDb.Customer.Name);
            Assert.Equal(cart.Customer.Email, cartDb.Customer.Email);
            Assert.Equal(cart.Customer.Phone, cartDb.Customer.Phone);
            Assert.Equal(cart.Customer.Address.Country, cartDb.Customer.Address.Country);
            Assert.Equal(cart.Customer.Address.State, cartDb.Customer.Address.State);
            Assert.Equal(cart.Customer.Address.City, cartDb.Customer.Address.City);
            Assert.Equal(cart.Customer.Address.Street, cartDb.Customer.Address.Street);
            Assert.Equal(cart.Customer.Address.PostalCode, cartDb.Customer.Address.PostalCode);
            Assert.Equal(cart.Customer.Address.District, cartDb.Customer.Address.District);
            Assert.Equal(cart.Customer.Address.Number, cartDb.Customer.Address.Number);
            Assert.Equal(cart.Customer.Address.Complement, cartDb.Customer.Address.Complement);
            Assert.Equal(cart.Products.Count(), cartDb.Products.Count());
            foreach (var product in cartDb.Products)
            {
                Assert.Equal(cart.Products.FirstOrDefault(p => p.Id == product.Id).Id, product.Id);
                Assert.Equal(cart.Products.FirstOrDefault(p => p.Id == product.Id).Name, product.Name);
                Assert.Equal(cart.Products.FirstOrDefault(p => p.Id == product.Id).Price, product.Price);
                Assert.Equal(cart.Products.FirstOrDefault(p => p.Id == product.Id).Quantity, product.Quantity);
            }
        }


        [Fact(DisplayName = nameof(GetAllCart))]
        [Trait("Integration - Infra.Data", "Cart Repositorio")]

        public async Task GetAllCart()
        {
            var carts = FakerCarts();

            foreach(var cart in carts)
            {
                await _repositoryProduct.AddAsync(cart, CancellationToken.None);
            }   
            var cartDbs = await _repositoryProduct.GetAllAsync();
            foreach(var cartDb in cartDbs)
            {
                var cart = carts.FirstOrDefault(c => c.Id == cartDb.Id);

                Assert.NotNull(cartDb);
                Assert.Equal(cartDb.Customer.Id, cart.Customer.Id);
                Assert.Equal(cartDb.Customer.Name, cart.Customer.Name);
                Assert.Equal(cartDb.Customer.Email, cart.Customer.Email);
                Assert.Equal(cartDb.Customer.Phone, cart.Customer.Phone);
                Assert.Equal(cartDb.Customer.Address.Country, cart.Customer.Address.Country);
                Assert.Equal(cartDb.Customer.Address.State, cart.Customer.Address.State);
                Assert.Equal(cartDb.Customer.Address.City, cart.Customer.Address.City);
                Assert.Equal(cartDb.Customer.Address.Street, cart.Customer.Address.Street);
                Assert.Equal(cartDb.Customer.Address.PostalCode, cart.Customer.Address.PostalCode);
                Assert.Equal(cartDb.Customer.Address.District, cart.Customer.Address.District);
                Assert.Equal(cartDb.Customer.Address.Number, cart.Customer.Address.Number);
                Assert.Equal(cartDb.Customer.Address.Complement, cart.Customer.Address.Complement);
            }
        }
    }
}
