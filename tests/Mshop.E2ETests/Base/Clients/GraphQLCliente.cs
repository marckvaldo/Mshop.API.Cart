using Mshop.Cart.E2ETests.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Mshop.E2ETests.Base.Clients
{
    public class GraphQLCliente
    {
        protected HttpClient _httpClient;

        public GraphQLCliente(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<T> SendQueryAsync<T>(string query)
        {
            var queryGraphQL = new
            {
                query
            };

            var content = new StringContent(JsonSerializer.Serialize(queryGraphQL), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(ConfigurationTests.URI_GRAPHQL, content);
            response.EnsureSuccessStatusCode();

            var options = new JsonSerializerOptions
            {              
                PropertyNameCaseInsensitive = true          
            };

            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(responseContent,options);
            
        }
    }
}
