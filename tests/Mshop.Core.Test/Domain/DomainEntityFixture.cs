using Bogus.Extensions;
using Mshop.Core.Test.Common;
using Mshop.Domain.Entity;

namespace Mshop.Core.Test.Domain
{
    public class DomainEntityFixture : BaseFixture
    {
        public DomainEntityFixture() : base()
        {

        }

        public static Product FakerProduct()
        {
            return new Product(
                Guid.NewGuid(),
                fakerStatic.Commerce.ProductDescription(),
                fakerStatic.Commerce.ProductName(),
                decimal.Parse(fakerStatic.Random.Decimal2(1, 100).ToString("F2")),
                true,
                Guid.NewGuid(),
                fakerStatic.Commerce.Categories(0).ToString(),
                fakerStatic.Image.LoremFlickrUrl(),
                fakerStatic.Random.Int(1, 100));
        }

        public static Product FakerProduct(Guid categoryId)
        {
            return new Product(
                Guid.NewGuid(),
                fakerStatic.Commerce.ProductDescription(),
                fakerStatic.Commerce.ProductName(),
                decimal.Parse(fakerStatic.Random.Decimal2(1, 100).ToString("F2")),
                true,
                categoryId,
                fakerStatic.Commerce.Categories(0).ToString(),
                fakerStatic.Image.LoremFlickrUrl(),
                fakerStatic.Random.Int(1, 100));
        }

        public List<Product> FakerProducts(int quantity = 3)
        {
            var products = new List<Product>();

            for (int i = 0; i < quantity; i++)
            {
                products.Add(FakerProduct());
            }
            return products;
        }

        public List<Product> FakerProducts(int quantity, Guid categoryId)
        {
            var products = new List<Product>();

            for (int i = 0; i < quantity; i++)
            {
                products.Add(FakerProduct(categoryId));
            }
            return products;
        }

        public Customer FakerCustomerWithAddress()
        {
            var phone = _faker.Phone.PhoneNumber();
            if (phone.Contains("+"))
                phone = phone.Replace("+55 ", "");

            var customer = new Customer(
                Guid.NewGuid(),
                fakerStatic.Name.FirstName(),
                fakerStatic.Internet.Email(),
                phone);

            var adress = new Address(
                fakerStatic.Address.StreetAddress(),
                fakerStatic.Address.BuildingNumber(),
                fakerStatic.Address.StreetName(),
                fakerStatic.Address.StreetSuffix(),
                fakerStatic.Address.City(),
                fakerStatic.Address.State(),
                fakerStatic.Address.ZipCode(),
                fakerStatic.Address.Country());

            customer.AddAdress(adress);

            return customer;
        }

        public Cart FakerCart(bool withProducts = false, bool withCustomer = false, bool withPayment = false)
        {
            var customer = FakerCustomerWithAddress();
            var products = FakerProducts(3);
            

            var cart = new Cart(Guid.NewGuid());

            if (withProducts)
            {

                foreach (var product in products)
                {
                    cart.AddItem(product, 1);
                }
            }

            var payment = FakerPayment(PaymentMethod.CreditCard, PaymentStatus.Pending, cart.GetTotal());

            if (withCustomer)
                cart.UpdateCustomer(customer);

            if(withPayment)
                cart.AddPayment(payment);

            return cart;
        }

        public List<Cart> FakerCarts(int quantity = 3)
        {
            var carts = new List<Cart>();
            for (int i = 0; i < quantity; i++)
            {
                carts.Add(FakerCart(true, true));
            }
            return carts;
        }

        public Address FakerAddress()
        {
            return new Address(_faker.Address.StreetName(), _faker.Address.BuildingNumber(), _faker.Random.Word(), _faker.Address.County(), _faker.Address.City(),
                _faker.Address.StateAbbr(), _faker.Address.ZipCode(), _faker.Address.Country());
        }

        public Customer FakerCustomer()
        {
            var phone = _faker.Phone.PhoneNumber();
            if (phone.Contains("+"))
                phone = phone.Replace("+55 ", "");

            return new Customer(
                Guid.NewGuid(),
                _faker.Person.FullName,
                _faker.Person.Email,
                phone
            );
        }

        public Customer FakerCustomerInvalid()
        {
            return new Customer(
                Guid.NewGuid(),
                "",
                _faker.Person.Email,
                _faker.Phone.PhoneNumber()
            );
        }


        public Payment FakerPayment(PaymentMethod paymentMethod, PaymentStatus paymentStatus = PaymentStatus.Pending ,decimal amountt = 0)
        {
            if(amountt == 0)
                amountt = fakerStatic.Finance.Amount(1, 100);

            if (PaymentMethod.CreditCard == paymentMethod || PaymentMethod.DebitCard == paymentMethod)
            {
                var payment =  new Payment(amountt, PaymentMethod.CreditCard, 1, fakerStatic.Finance.CreditCardNumber());
                payment.UpdateStatus(paymentStatus);
                return payment;
            }

            if (PaymentMethod.BoletoBancario == paymentMethod)
            {
                var payment =new Payment(amountt, PaymentMethod.BoletoBancario, fakerStatic.Finance.CreditCardNumber().ToString(), DateTime.UtcNow.AddDays(5));
                payment.UpdateStatus(paymentStatus);
                return payment;
            }

            if (PaymentMethod.Pix == paymentMethod || PaymentMethod.Other == paymentMethod)
            {
                var payment = new Payment(amountt, PaymentMethod.Pix);
                payment.UpdateStatus(paymentStatus);
                return payment;
            }

            return new Payment(fakerStatic.Finance.Amount(1, 100), PaymentMethod.Pix);
        }
    }
}
