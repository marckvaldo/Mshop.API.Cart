using Mshop.Application;
using Mshop.API.GraphQL.Cart.GraphQL.Cart;
using Mshop.Infra.Consumer;
using Mshop.Infra.Data;
using MShop.Broker.Cart;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services
    .AddConsumer(builder.Configuration)
    .AddCache(builder.Configuration)
    .AddDataBaseAndRepository(builder.Configuration)
    .AddRabbitMQ(builder.Configuration)
    .AddAplication(builder.Configuration)
    .AddGraphQLServer()
    .AddQueryType()
    .AddTypeExtension<CartQueries>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.CreateIndexMongoDB();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapGraphQL();
app.MapControllers();
app.Run();

namespace Mshop.API.GraphQL.Cart
{
    public partial class Program
    {
        // This class is intentionally left empty. It is used to allow the use of partial classes.
    }
}