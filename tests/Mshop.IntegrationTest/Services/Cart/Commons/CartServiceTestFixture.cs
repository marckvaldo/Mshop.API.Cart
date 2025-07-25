﻿using Mshop.Application.Commons.DTO;
using Mshop.Domain.Entity;
using Mshop.Infra.Data.Context;
using Mshop.IntegrationTest.Common;
using Mshop.IntegrationTest.Common.Persistence.DTOs;
using Mshop.IntegrationTest.Common.Persistence.MongoDb.Cart;
using Mshop.IntegrationTest.Common.Persistence.Mysql.Category;
using Mshop.IntegrationTest.Common.Persistence.Mysql.Product;
using Mshop.IntegrationTest.Infra.configuration;

namespace Mshop.IntegrationTest.Services.Cart.Commons;

public class CartServiceTestFixture : IntegrationBaseFixture
{
    protected readonly ProductPersistence _productPersistenceDabaBase;
    protected readonly CategoryPersistence _categoryPersistenceDataBase;
    protected readonly CartPersistence _cartPersistence;
    public CartServiceTestFixture() : base()
    {
        _productPersistenceDabaBase = new ProductPersistence(ConfigurationTests.ConnectionStringMysql);
        _categoryPersistenceDataBase = new CategoryPersistence(ConfigurationTests.ConnectionStringMysql);
        _cartPersistence = new CartPersistence(new MongoDbContext(ConfigurationTests.MongoDb, ConfigurationTests.DataBaseMongoDb));
    }

    public bool PersistProductsDataBase()
    {

        var category = new CategoryPersistenceDTO { IsActive = true, Id = Guid.NewGuid(), Name = _faker.Commerce.Categories(1)[0] };
        _categoryPersistenceDataBase.AddCategoryAsync(category).Wait();

        var produtct = FakerProducts(10, category.Id);
        foreach (var produto in produtct)
        {
            var produtoDTO = new ProductsPersistenceDTO();
            produtoDTO.Description = produto.Description;
            produtoDTO.Name = produto.Name;
            produtoDTO.Price = produto.Price;
            produtoDTO.Stock = 10;
            produtoDTO.IsActive = true;
            produtoDTO.IsSale = produto.IsSale;
            produtoDTO.CategoryId = produto.CategoryId;
            produtoDTO.Thumb = produto.Thumb;
            produtoDTO.Id = produto.Id;
            _productPersistenceDabaBase.AddProductAsync(produtoDTO).Wait();
        }

        return true;
    }

    public async Task ClearDataBase()
    {
        _productPersistenceDabaBase.DeleteAllProductAsync().Wait();
        _categoryPersistenceDataBase.DeleteAllCategoryAsync().Wait();
        _cartPersistence.DeleteAllCartAsync(new CancellationToken()).Wait();
    }

    public Task<IEnumerable<ProductsPersistenceDTO>> GetAllProductsMysqlAsync()
    {
        return _productPersistenceDabaBase.GetAllProductsAsync();
    }


    public PaymentDTO PaymentToPaymamentDTO(Payment payment)
    {
        return new PaymentDTO(
                payment.Amount,
                payment.PaymentMethod,
                payment.Status,
                payment.Installments,
                payment.CardToken,
                payment.BoletoNumber,
                payment.BoletoDueDate);
    }
}

