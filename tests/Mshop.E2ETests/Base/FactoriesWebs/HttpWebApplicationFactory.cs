using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Mshop.E2ETests.Base.FactoriesWebs
{
    public class HttpWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            var environment = "E2ETests";
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", environment); //setando globalmente
            builder.UseEnvironment(environment);

            // You can override this method to configure the web host for your tests.
            // For example, you can set up a test database or mock services.
            base.ConfigureWebHost(builder);
        }

    }
}
