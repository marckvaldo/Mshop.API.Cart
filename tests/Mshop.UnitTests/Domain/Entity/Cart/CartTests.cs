using Mshop.Core.Message;
using Mshop.Domain.Entity;
using Mshop.Domain.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mshop.UnitTests.Domain.Entity.Cart
{
    public class CartTests : CategoryTesteFixture
    {
        private INotification _notification;
        public CartTests() : base()
        {
           _notification = new Notifications();
        }

        [Fact(DisplayName = nameof(Cart_ShouldInitializeWithEmptyProductList))]
        [Trait("Domain", "Cart")]
        public void Cart_ShouldInitializeWithEmptyProductList()
        {
            // Arrange & Act
            var cart = new Mshop.Domain.Entity.Cart(Guid.Empty);
            cart.IsValid(_notification);

            // Assert
            Assert.Empty(cart.Products);
            Assert.False(_notification.HasErrors());
            Assert.Equal(cart.Status, CartStatus.PendingCheckout);
        }

        [Fact(DisplayName = nameof(Cart_ShouldAddItemToProductList))]
        [Trait("Domain", "Cart")]
        public void Cart_ShouldAddItemToProductList()
        {
            // Arrange
            var cart = new Mshop.Domain.Entity.Cart(Guid.Empty);
            var product = FakerProduct();

            // Act
            cart.AddItem(product, 1);
            cart.IsValid(_notification);

            // Assert
            Assert.Single(cart.Products);
            Assert.Equal(1, cart.Products[0].Quantity);
            Assert.False(_notification.HasErrors());
            Assert.Equal(cart.Status, CartStatus.PendingCheckout);
        }

        [Fact(DisplayName = nameof(Cart_ShouldUpdateQuantityWhenAddingExistingProduct))]
        [Trait("Domain", "Cart")]
        public void Cart_ShouldUpdateQuantityWhenAddingExistingProduct()
        {
            // Arrange
            var cart = new Mshop.Domain.Entity.Cart(Guid.Empty);
            var product = FakerProduct();
            cart.AddItem(product, 1);

            // Act
            cart.AddItem(product, 2);
            cart.IsValid(_notification);

            // Assert
            Assert.Single(cart.Products);
            Assert.Equal(3, cart.Products[0].Quantity);
            Assert.False(_notification.HasErrors());
            Assert.Equal(cart.Status, CartStatus.PendingCheckout);
            Assert.True(cart.Events.Count == 2);
            Assert.IsType<OrderItemAddedEvent>(cart.Events.Where(c => c.EventName == nameof(OrderItemAddedEvent)).FirstOrDefault());
            Assert.IsType<OrderItemModifiedEvent>(cart.Events.Where(c => c.EventName == nameof(OrderItemModifiedEvent)).FirstOrDefault());
        }

        [Fact(DisplayName = nameof(Cart_ShouldRemoveQuantity))]
        [Trait("Domain", "Cart")]
        public void Cart_ShouldRemoveQuantity()
        {
            // Arrange
            var cart = new Mshop.Domain.Entity.Cart(Guid.Empty);
            var product = FakerProduct();
            cart.AddItem(product, 2);

            // Act
            cart.RemoveQuantity(product.Id);
            cart.IsValid(_notification);

            // Assert
            Assert.Single(cart.Products);
            Assert.Equal(1, cart.Products[0].Quantity);
            Assert.False(_notification.HasErrors());
            Assert.Equal(cart.Status, CartStatus.PendingCheckout);

            Assert.True(cart.Events.Count == 2);
            Assert.IsType<OrderItemAddedEvent>(cart.Events.Where(c => c.EventName == nameof(OrderItemAddedEvent)).FirstOrDefault());
            Assert.IsType<OrderItemModifiedEvent>(cart.Events.Where(c => c.EventName == nameof(OrderItemModifiedEvent)).FirstOrDefault());
        }

        [Fact(DisplayName = nameof(Cart_ShouldRemoveItemWhenQuantityIsZero))]
        [Trait("Domain", "Cart")]
        public void Cart_ShouldRemoveItemWhenQuantityIsZero()
        {
            // Arrange
            var cart = new Mshop.Domain.Entity.Cart(Guid.Empty);
            var product = FakerProduct();
            cart.AddItem(product, 1);

            // Act
            cart.RemoveQuantity(product.Id);
            cart.IsValid(_notification);

            // Assert
            Assert.Empty(cart.Products);
            Assert.False(_notification.HasErrors());
            Assert.Equal(cart.Status, CartStatus.PendingCheckout);

            Assert.True(cart.Events.Count == 2);
            Assert.IsType<OrderItemAddedEvent>(cart.Events.Where(c => c.EventName == nameof(OrderItemAddedEvent)).FirstOrDefault());
            Assert.IsType<OrderItemRemovedEvent>(cart.Events.Where(c => c.EventName == nameof(OrderItemRemovedEvent)).FirstOrDefault());
        }

        [Fact(DisplayName = nameof(Cart_ShouldClearAllItems))]
        [Trait("Domain", "Cart")]
        public void Cart_ShouldClearAllItems()
        {
            // Arrange
            var cart = new Mshop.Domain.Entity.Cart(Guid.Empty);
            var product = FakerProduct();
            cart.AddItem(product, 1);

            // Act
            cart.ClearCart();
            cart.IsValid(_notification);

            // Assert
            Assert.Empty(cart.Products);
            Assert.False(_notification.HasErrors());
            Assert.Equal(cart.Status, CartStatus.PendingCheckout);

            Assert.True(cart.Events.Count == 2);
            Assert.IsType<OrderItemAddedEvent>(cart.Events.Where(c => c.EventName == nameof(OrderItemAddedEvent)).FirstOrDefault());
            Assert.IsType<OrderItensRemovedEvent>(cart.Events.Where(c => c.EventName == nameof(OrderItensRemovedEvent)).FirstOrDefault());
        }

        [Fact(DisplayName = nameof(Cart_ShouldRemoveItemById))]
        [Trait("Domain", "Cart")]
        public void Cart_ShouldRemoveItemById()
        {
            // Arrange
            var cart = new Mshop.Domain.Entity.Cart(Guid.Empty);
            var product = FakerProduct();
            cart.AddItem(product, 1);

            // Act
            cart.RemoveItem(product.Id);
            cart.IsValid(_notification);

            // Assert
            Assert.Empty(cart.Products);
            Assert.False(_notification.HasErrors());
            Assert.Equal(cart.Status, CartStatus.PendingCheckout);

            Assert.True(cart.Events.Count == 2);
            Assert.IsType<OrderItemAddedEvent>(cart.Events.Where(c => c.EventName == nameof(OrderItemAddedEvent)).FirstOrDefault());
            Assert.IsType<OrderItemRemovedEvent>(cart.Events.Where(c => c.EventName == nameof(OrderItemRemovedEvent)).FirstOrDefault());
        }

        [Fact(DisplayName = nameof(Cart_ShouldReturnTotalPrice))]
        [Trait("Domain", "Cart")]
        public void Cart_ShouldReturnTotalPrice()
        {
            // Arrange
            var cart = new Mshop.Domain.Entity.Cart(Guid.Empty);
            var products = FakerProducts(5);
            
            foreach (var item in products)
            {
                cart.AddItem(item, 1);
            }
            cart.IsValid(_notification);

            decimal totalProducts = cart.Products.Sum(p => p.Quantity * p.Price);

            // Act
            var total = cart.GetTotal();

            // Assert
            Assert.Equal(totalProducts, total);
            Assert.False(_notification.HasErrors());
            Assert.Equal(cart.Status, CartStatus.PendingCheckout);

            Assert.True(cart.Events.Count == 5);
            Assert.IsType<OrderItemAddedEvent>(cart.Events.Where(c => c.EventName == nameof(OrderItemAddedEvent)).FirstOrDefault());
            //Assert.IsType<OrderItemRemovedEvent>(cart.Events.Where(c => c.EventName == nameof(OrderItemRemovedEvent)).FirstOrDefault());
        }

        [Fact(DisplayName = nameof(Cart_ShouldUpdateCustomer))]
        [Trait("Domain", "Cart")]
        public void Cart_ShouldUpdateCustomer()
        {
            // Arrange
            var cart = new Mshop.Domain.Entity.Cart(Guid.Empty);
            var customer = FakerCustomer();

            // Act
            cart.UpdateCustomer(customer);
            cart.IsValid(_notification);

            // Assert
            Assert.Equal(customer, cart.Customer);
            Assert.False(_notification.HasErrors());
            Assert.Equal(cart.Status, CartStatus.PendingCheckout);

            Assert.True(cart.Events.Count == 1);
            Assert.IsType<OrderModifiedEvent>(cart.Events.Where(c => c.EventName == nameof(OrderModifiedEvent)).FirstOrDefault());
        }

        [Fact(DisplayName = nameof(Cart_ShouldAddPayment))]
        [Trait("Domain", "Cart")]
        public void Cart_ShouldAddPayment()
        {
            // Arrange
            var fakerProducts = FakerProducts(3);
            var cart = new Mshop.Domain.Entity.Cart(Guid.Empty);
            foreach (var item in fakerProducts)
            {
                cart.AddItem(item, 1);
            }

            var payment = FakerPayment(PaymentMethod.CreditCard,PaymentStatus.Pending,fakerProducts.Sum(x=>x.Total));
            cart.AddPayment(payment);
            cart.IsValid(_notification);
            // Assert
            Assert.Single(cart.Payments);
            Assert.Equal(payment, cart.Payments[0]);
            Assert.False(_notification.HasErrors());
            Assert.Equal(cart.GetAmount(), payment.Amount);
            Assert.Equal(cart.Status, CartStatus.PendingCheckout);

            Assert.True(cart.Events.Count == 4);
            Assert.IsType<OrderItemAddedEvent>(cart.Events.Where(c => c.EventName == nameof(OrderItemAddedEvent)).FirstOrDefault());
            Assert.IsType<OrderModifiedEvent>(cart.Events.Where(c => c.EventName == nameof(OrderModifiedEvent)).FirstOrDefault());
        }



        [Fact(DisplayName = nameof(Cart_ShouldReturnFalseWhenPaymentNotFound))]
        [Trait("Domain", "Cart")]
        public void Cart_ShouldReturnFalseWhenPaymentNotFound()
        {
            // Arrange
            var fakerProducts = FakerProducts(3);
            var cart = new Mshop.Domain.Entity.Cart(Guid.Empty);
            foreach (var item in fakerProducts)
            {
                cart.AddItem(item, 1);
            }

            var payment = FakerPayment(PaymentMethod.CreditCard, PaymentStatus.Pending, 0);
            cart.AddPayment(payment);
            cart.IsValid(_notification);
            // Assert
            Assert.Single(cart.Payments);
            Assert.Equal(payment, cart.Payments[0]);
            Assert.True(_notification.HasErrors());
            Assert.Equal(cart.GetAmount(), payment.Amount);
            Assert.Equal(cart.Status, CartStatus.PendingCheckout);

            Assert.True(cart.Events.Count == 4);
            Assert.IsType<OrderItemAddedEvent>(cart.Events.Where(c => c.EventName == nameof(OrderItemAddedEvent)).FirstOrDefault());
            Assert.IsType<OrderModifiedEvent>(cart.Events.Where(c => c.EventName == nameof(OrderModifiedEvent)).FirstOrDefault());
        }


        [Fact(DisplayName = nameof(Cart_ShouldReturnFalseWhenPutStatusCheckoutCompletAndNotFoundPayment))]
        [Trait("Domain", "Cart")]
        public void Cart_ShouldReturnFalseWhenPutStatusCheckoutCompletAndNotFoundPayment()
        {
            // Arrange
            var fakerProducts = FakerProducts(3);
            var cart = new Mshop.Domain.Entity.Cart(Guid.Empty);
            foreach (var item in fakerProducts)
            {
                cart.AddItem(item, 1);
            }

            cart.Checkout();
            cart.IsValid(_notification);
            // Assert
            Assert.True(_notification.HasErrors());
            Assert.NotEqual(cart.GetAmount(), cart.GetTotal());
            Assert.Equal(cart.Status, CartStatus.PendingCheckout);

            Assert.True(cart.Events.Count == 3);
            Assert.IsType<OrderItemAddedEvent>(cart.Events.Where(c => c.EventName == nameof(OrderItemAddedEvent)).FirstOrDefault());

        }


    }
}
