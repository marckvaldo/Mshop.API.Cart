using Mshop.Infra.Consumer.Cache;
using NRedisStack.RedisStackCommands;
using NRedisStack.Search;
using NRedisStack.Search.Literals.Enums;
using StackExchange.Redis;

namespace Mshop.IntegrationTest.Common.Persistence.Cache
{
    public class StartIndex
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly IDatabase _database;

        public StartIndex(IConnectionMultiplexer redis)
        {
            _redis = redis;
            _database = _redis.GetDatabase();
        }

        public async Task CreateIndex()
        {
            var ft = _database.FT();
            
            var indexNameProduct = $"{IndexName.Product}Index";
            var prefixProduct = $"{IndexName.Product}:";


            try
            {
                var result = await ft.InfoAsync(indexNameProduct);
            }
            catch (Exception ex)
            {
                await ft.CreateAsync(indexNameProduct,
                   new FTCreateParams().On(IndexDataType.HASH).Prefix(prefixProduct),
                   new Schema()
                       .AddTextField("Id")
                       .AddTextField("Name", 5.0)
                       .AddTextField("Description")
                       .AddNumericField("Price")
                       .AddNumericField("Stock")
                       .AddTagField("IsActive")
                       .AddTagField("IsSale")
                       .AddTagField("CategoryId")
                       .AddTextField("Category")
                       .AddTextField("Thumb")
                   );
            }


            var indexNameCategory = $"{IndexName.Category}Index";
            var prefixCategory = $"{IndexName.Category}:";

            try
            {
                var result = await ft.InfoAsync(indexNameCategory);
            }
            catch (Exception ex)
            {
                await ft.CreateAsync(indexNameCategory,
                   new FTCreateParams().On(IndexDataType.HASH).Prefix(prefixCategory),
                   new Schema()
                       .AddTextField("Id")
                       .AddTextField("Name", 5.0)
                       .AddTagField("IsActive")
                   );
            }


            var indexNameImages = $"{IndexName.Image}Index";
            var prefixImages = $"{IndexName.Image}:";

            try
            {
                var result = await ft.InfoAsync(indexNameImages);
            }
            catch (Exception ex)
            {
                await ft.CreateAsync(indexNameImages,
                   new FTCreateParams().On(IndexDataType.HASH).Prefix(prefixImages),
                   new Schema()
                       .AddTextField("Id")
                       .AddTextField("FileName", 5.0)
                       .AddTagField("ProductId")
                   );
            }



        }

        public async Task DeleteIndex()
        {
            var db = _redis.GetDatabase(); var ft = db.FT(); 
            
            // Nome dos índices a serem excluídos
            var indexNameProduct = $"{IndexName.Product}Index"; 
            var indexNameCategory = $"{IndexName.Category}Index";

            // Excluindo os índices
            try
            {
                await ft.DropIndexAsync(indexNameProduct);
                await ft.DropIndexAsync(indexNameCategory);
            }
            catch (Exception ex)
            {
            }
        }
    }
}
