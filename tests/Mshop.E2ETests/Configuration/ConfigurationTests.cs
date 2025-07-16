using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mshop.Cart.E2ETests.Configuration
{
    public static class ConfigurationTests
    {
        //public const string ConnectionString = "mongodb://admin:admin123@localhost:27017";
        //public const string DataBase = "MshopDbTest";
        //public const string GrpcProduct = "http://localhost:90";
        //public const string GrpcCustomer = "https://localhost:5002";
        public const string ConnectionStringMysql = "Server=localhost;Port=3308;Database=mshop;User id=mshop;Password=mshop;Convert Zero Datetime=True";
        
        //public const string EndpointCache = "localhost:8378";
        //public const string PasswordCache = "";
        //public const string UserCache = "";

        /*public const string MongoDb = "mongodb://admin:admin123@localhost:27017";
        public const string DataBaseMongoDb = "MshopDb";*/


        public const string HostNameRabbitMQ = "localhost";
        public const string UserNameRabbitMQ = "admin";
        public const string PasswordRabbitMQ = "123456";
        public const string ExchangeRabbitMQ = "orders.Events";
        public const string PortRabbitMQ = "5672";
        public const string VhostRabbitMQ = "Mshop";
        public const string QueueOrderRabbitMQ = "order.processing.V1";
        public const bool DurableRabbitMQ = true;


        public static string URL_API_CART = "/api/v1/cart";

        public static string URI_GRAPHQL = "http://localhost:5000/graphql";

    }
}
