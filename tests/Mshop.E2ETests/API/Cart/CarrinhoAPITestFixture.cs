using Mshop.Cart.E2ETests.Configuration;
using Mshop.E2ETests.Base;
using Mshop.E2ETests.Persistence.DTOs;
using Mshop.E2ETests.Persistence.Mysql.Category;
using Mshop.E2ETests.Persistence.Mysql.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mshop.E2ETests.API.Cart
{
    public class CarrinhoAPITestFixture : BaseWebApplication
    {
        protected CategoryPersistence _categoryPersistenceDataBase;
        protected ProductPersistence _productPersistenceDabaBase;
        public CarrinhoAPITestFixture() : base()
        {
            _categoryPersistenceDataBase = new CategoryPersistence(ConfigurationTests.ConnectionStringMysql);
            _productPersistenceDabaBase = new ProductPersistence(ConfigurationTests.ConnectionStringMysql);
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

        public Task<IEnumerable<ProductsPersistenceDTO>> GetAllProductsMysqlAsync()
        {
            return _productPersistenceDabaBase.GetAllProductsAsync();
        }
    }
}
