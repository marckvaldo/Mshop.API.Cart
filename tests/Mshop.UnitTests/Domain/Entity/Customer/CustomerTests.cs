using FluentValidation.Results;
using Moq;
using DomainEntity = Mshop.Domain.Entity;
using Mshop.Domain.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mshop.Core.Message;

namespace Mshop.UnitTests.Domain.Entity.Customer
{
    public class CustomerTests : CustomerTestsFixture
    {
        public CustomerTests() : base()
        {
            
        }

        [Fact(DisplayName = nameof(Customer_Constructor_ShouldAssignValues))]
        [Trait("Domain", "Customer")]
        public async Task Customer_Constructor_ShouldAssignValues()
        {
            // Arrange
            var name = _faker.Person.FullName;
            var email = _faker.Person.Email;
            var phone = _faker.Phone.PhoneNumber();

            // Act
            var customer = new DomainEntity.Customer(Guid.NewGuid(), name, email, phone);

            // Assert
            Assert.Equal(name, customer.Name);
            Assert.Equal(email, customer.Email);
            Assert.Equal(phone, customer.Phone);
        }

        [Fact(DisplayName = nameof(Customer_AddAdress_ShouldAssignAddress))]
        [Trait("Domain", "Customer")]
        public async Task Customer_AddAdress_ShouldAssignAddress()
        {
            // Arrange
            var notification = new Notifications();
            var customer = FakerCustomer();
            customer.IsValid(notification);

            var address = FakerAddress();
            address.IsValid(notification);

            // Act
            customer.AddAdress(address);
           

            // Assert
            Assert.Equal(address, customer.Address);
            Assert.False(notification.HasErrors());
        }

        [Fact(DisplayName = nameof(Customer_IsValid_ShouldReturnTrue_WhenCustomerIsValid))]
        [Trait("Domain", "Customer")]
        public async Task Customer_IsValid_ShouldReturnTrue_WhenCustomerIsValid()
        {
            // Arrange
            var notification = new Notifications();
            var customer = FakerCustomer();

            // Act
            var isValid = customer.IsValid(notification);

            // Assert
            Assert.True(isValid);
            
            
        }

        [Fact(DisplayName = nameof(Customer_IsValid_ShouldReturnFalse_WhenCustomerIsInvalid))]
        [Trait("Domain", "Customer")]
        public async Task Customer_IsValid_ShouldReturnFalse_WhenCustomerIsInvalid()
        {
            // Arrange
            var notification = new Notifications();
            var customer = FakerCustomerInvalid();
            
            // Act
            var isValid = customer.IsValid(notification);

            // Assert
            Assert.False(isValid);
           
        }
    }
}
