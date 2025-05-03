using Dapper;
using Mshop.IntegrationTest.Common.Persistence.DTOs;
using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mshop.IntegrationTest.Common.Persistence.Mysql.Product
{
    public class ProductPersistence
    {
        private readonly string _connectionString;

        public ProductPersistence(string connectionString)
        {
            _connectionString = connectionString;
        }

        private IDbConnection CreateConnection()
        {
            return new MySqlConnection(_connectionString);
        }

        public async Task<ProductsPersistenceDTO> GetProductByIdAsync(Guid productId)
        {
            using var connection = CreateConnection();
            var query = "SELECT * FROM Products WHERE Id = @Id";
            var parameters = new { Id = productId };
            var product = await connection.QuerySingleOrDefaultAsync<ProductsPersistenceDTO>(query, parameters);
            return product;
        }

        public async Task<IEnumerable<ProductsPersistenceDTO>> GetAllProductsAsync()
        {
            using var connection = CreateConnection();
            var query = "SELECT * FROM Products";
            var products = await connection.QueryAsync<ProductsPersistenceDTO>(query);
            return products;
        }

        public async Task AddProductAsync(ProductsPersistenceDTO product)
        {
            using var connection = CreateConnection();

            var query = "INSERT INTO Products (Id, Name, Price, Description, Stock, IsActive, IsSale, CategoryId, Thumb) " +
                "VALUES (@Id, @Name, @Price, @Description, @Stock, @IsActive, @IsSale, @CategoryId, @Thumb)";

            var parameters = new
            {
                product.Id,
                product.Name,
                product.Price,
                product.Description,
                product.Stock,
                product.IsActive,
                product.IsSale,
                product.CategoryId,
                product.Thumb
            };

            await connection.ExecuteAsync(query, parameters);
        }

        public async Task UpdateProductAsync(ProductsPersistenceDTO product)
        {
            using var connection = CreateConnection();
            var query = "UPDATE Products SET Name = @Name, Price = @Price WHERE Id = @Id";
            var parameters = new { product.Id, product.Name, product.Price };
            await connection.ExecuteAsync(query, parameters);
        }

        public async Task DeleteProductAsync(Guid productId)
        {
            using var connection = CreateConnection();
            var query = "DELETE FROM Products WHERE Id = @Id";
            var parameters = new { Id = productId };
            await connection.ExecuteAsync(query, parameters);
        }

        public async Task DeleteAllProductAsync()
        {
            using var connection = CreateConnection();
            var query = "DELETE FROM Products";
            await connection.ExecuteAsync(query);
        }
    }
}
