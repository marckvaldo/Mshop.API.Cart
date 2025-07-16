using Dapper;
using Mshop.E2ETests.Persistence.DTOs;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mshop.E2ETests.Persistence.Mysql.Category
{
    public class CategoryPersistence
    {
        private readonly string _connectionString;

        public CategoryPersistence(string connectionString)
        {
            _connectionString = connectionString;
        }

        private IDbConnection CreateConnection()
        {
            return new MySqlConnection(_connectionString);
        }

        public async Task<CategoryPersistenceDTO> GetCategoryByIdAsync(Guid categoryId)
        {
            using var connection = CreateConnection();
            var query = "SELECT * FROM Categories WHERE Id = @Id";
            var parameters = new { Id = categoryId };
            var category = await connection.QuerySingleOrDefaultAsync<CategoryPersistenceDTO>(query, parameters);
            return category;
        }

        public async Task<IEnumerable<CategoryPersistenceDTO>> GetAllCategoriesAsync()
        {
            using var connection = CreateConnection();
            var query = "SELECT * FROM Categories";
            var categories = await connection.QueryAsync<CategoryPersistenceDTO>(query);
            return categories;
        }

        public async Task AddCategoryAsync(CategoryPersistenceDTO category)
        {
            using var connection = CreateConnection();
            var query = "INSERT INTO Categories (Id, Name, IsActive) VALUES (@Id, @Name, @IsActive)";
            var parameters = new { category.Id, category.Name, category.IsActive };
            await connection.ExecuteAsync(query, parameters);
        }

        public async Task UpdateCategoryAsync(CategoryPersistenceDTO category)
        {
            using var connection = CreateConnection();
            var query = "UPDATE Categories SET Name = @Name, Price = @IsActive WHERE Id = @Id";
            var parameters = new { category.Id, category.Name, category.IsActive };
            await connection.ExecuteAsync(query, parameters);
        }

        public async Task DeleteCategoryAsync(Guid categoryId)
        {
            using var connection = CreateConnection();
            var query = "DELETE FROM Categories WHERE Id = @Id";
            var parameters = new { Id = categoryId };
            await connection.ExecuteAsync(query, parameters);
        }


        public async Task DeleteAllCategoryAsync()
        {
            using var connection = CreateConnection();
            var query = "DELETE FROM Categories";
            await connection.ExecuteAsync(query);
        }
    }
}
