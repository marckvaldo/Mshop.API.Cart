using Bogus;
using FluentValidation.Results;
using DomainEntity = Mshop.Domain.Entity;
using Mshop.Domain.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Mshop.Domain.Entity;
using Mshop.Core.Message;

namespace Mshop.UnitTests.Domain.Entity.Address
{
    public class AddressTests : AddressTesteFixture
    {
        public AddressTests() : base()
        {
            
        }

        [Fact(DisplayName = nameof(Address_Constructor_ShouldAssignValues))]
        [Trait("Domain", "Address")]
        public void Address_Constructor_ShouldAssignValues()
        {
            // Arrange
            var street = _faker.Address.StreetName();
            var number = _faker.Address.BuildingNumber();
            var complement = _faker.Random.Word();
            var neighborhood = _faker.Address.County();
            var city = _faker.Address.City();
            var state = _faker.Address.StateAbbr();
            var postalCode = _faker.Address.ZipCode();
            var country = _faker.Address.Country();

            // Act
            var address = new DomainEntity.Address(street, number, complement, neighborhood, city, state, postalCode, country);
            address.IsValid(new Core.Message.Notifications());

            // Assert
            Assert.Equal(street, address.Street);
            Assert.Equal(number, address.Number);
            Assert.Equal(complement, address.Complement);
            Assert.Equal(neighborhood, address.District);
            Assert.Equal(city, address.City);
            Assert.Equal(state, address.State);
            Assert.Equal(postalCode, address.PostalCode);
            Assert.Equal(country, address.Country);
        }


        [Fact(DisplayName = nameof(Address_IsValid_ShouldReturnTrue_WhenAddressIsValid))]
        [Trait("Domain", "Address")]
        public void Address_IsValid_ShouldReturnTrue_WhenAddressIsValid()
        {
            // Arrange
            var notification = new Notifications();
            var address = FakerAddress();

            // Act
            var isValid = address.IsValid(notification);

            // Assert
            Assert.True(isValid);
        }

        [Fact(DisplayName = nameof(Address_IsValid_ShouldReturnFalse_WhenAddressIsInvalid))]
        [Trait("Domain", "Address")]
        public void Address_IsValid_ShouldReturnFalse_WhenAddressIsInvalid()
        {
            // Arrange
            var notification = new Notifications();
            var address = new DomainEntity.Address(
                "", 
                _faker.Address.BuildingNumber(), 
                _faker.Random.Word(), 
                _faker.Address.County(), 
                _faker.Address.City(),
                _faker.Address.StateAbbr(), 
                _faker.Address.ZipCode(), 
                _faker.Address.Country()
            );

            // Act
            var result = address.IsValid(notification);

            // Assert
            Assert.False(result);
        }
    }
}
