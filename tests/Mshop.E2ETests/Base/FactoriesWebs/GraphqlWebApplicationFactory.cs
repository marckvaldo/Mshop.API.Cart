using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mshop.E2ETests.Base.FactoriesWebs
{
    public class GraphqlWebApplicationFactory<TStartup> : HttpWebApplicationFactory<TStartup> where TStartup : class
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
