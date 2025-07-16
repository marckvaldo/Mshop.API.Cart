using Mshop.API.Cart.Configuration;
using Mshop.Application;
using Mshop.Application.Services.Cart.Commands;
using Mshop.Infra.Consumer;
using Mshop.Infra.Data;
using MShop.API.Cart.Configuration;
using MShop.Broker.Cart;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddConfigurationController()
    .AddSwaggerConfiguration()
    .AddConfigurationModelState()
    .AddConsumer(builder.Configuration)
    .AddCache(builder.Configuration)
    .AddDataBaseAndRepository(builder.Configuration)
    .AddRabbitMQ(builder.Configuration)
    .AddAplication(builder.Configuration)
    .AddHealthChecks();

//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

var app = builder.Build();

app.CreateIndexMongoDB();
app.UseDocumentation();
app.AddMapHealthCheck();


// Configure the HTTP request pipeline.
/*if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}*/

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();

namespace Mshop.API.Cart
{
    public partial class Program
    {

    }
}