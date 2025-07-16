using Microsoft.Extensions.DependencyInjection;
using Mshop.Cart.E2ETests.Emun;
using Mshop.Core.Test.Common;
using Mshop.Core.Test.Domain;
using Mshop.E2ETests.Base.Clients;
using Mshop.E2ETests.Base.FactoriesWebs;

namespace Mshop.E2ETests.Base
{
    public class BaseWebApplication : DomainEntityFixture
    {
        protected HttpWebApplicationFactory<Mshop.API.Cart.Program> _webApp;
        protected GraphqlWebApplicationFactory<Mshop.API.GraphQL.Cart.Program> _graphQLWebApp;

        protected IServiceProvider _serviceProvider;
        protected HttpClient _httpClient;
        protected APIClient _apiClient;
        protected GraphQLCliente _graphQlClient;
        public BaseWebApplication(TypeProjetct typeProjetct = TypeProjetct.Http) : base()
        {
            if (typeProjetct == TypeProjetct.Http)
                BuildWebAplicationAPI().Wait();
            else if (typeProjetct == TypeProjetct.GraphQL)
                BuildWebAplicationGraphQL().Wait();
            else
                throw new ArgumentException("Tipo de projeto não suportado.");

        }

        protected async Task BuildWebAplicationAPI()
        {
            _webApp = new HttpWebApplicationFactory<Mshop.API.Cart.Program>();
            _serviceProvider = _webApp.Services.GetRequiredService<IServiceProvider>();
            _httpClient = _webApp.CreateClient();
            _apiClient = new APIClient(_httpClient);
        }

        protected async Task BuildWebAplicationGraphQL()
        {
            _graphQLWebApp = new GraphqlWebApplicationFactory<Mshop.API.GraphQL.Cart.Program>();
            _serviceProvider = _graphQLWebApp.Services.GetRequiredService<IServiceProvider>();
            _httpClient = _graphQLWebApp.CreateClient();
            _graphQlClient = new GraphQLCliente(_httpClient);
        }
    }
}
