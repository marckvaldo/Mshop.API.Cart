using Microsoft.Extensions.DependencyInjection;
using Mshop.Cart.E2ETests.Emun;
using Mshop.Core.Test.Common;
using Mshop.E2ETests.Base.Clients;
using Mshop.E2ETests.Base.FactoriesWebs;

namespace Mshop.E2ETests.Base
{
    public class BaseWebApplication : BaseFixture
    {
        protected HttpWebApplicationFactory<Mshop.API.Cart.Program> _webApp;

        protected IServiceProvider _serviceProvider;
        protected HttpClient _httpClient;
        protected APIClient _apiClient;
        public BaseWebApplication(TypeProjetct typeProjetct = TypeProjetct.Http) : base()
        {
            if(typeProjetct == TypeProjetct.Http)
                BuildWebAplicationAPI().Wait();
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
    }
}
