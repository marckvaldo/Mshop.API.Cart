using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MShop.API.Cart.Configuration
{
    public static class ConfigurationSwaggerConfig
    {
        public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
        {
           
            services.AddSwaggerGen(c =>
            {
                c.OperationFilter<SwaggerDefaultValues>();
            });

            services.ConfigureOptions<SwaggerConfig>();

            //colcoando versionamento na API
            //precisa instalar 2 pacotes
            services.AddApiVersioning(options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ReportApiVersions = true;

            }).AddMvc().AddApiExplorer(setup =>
            {
                setup.GroupNameFormat = "'v'VVV";
                setup.SubstituteApiVersionInUrl = true;
            });



            //fim do versionamento



            return services;
        }

        public static WebApplication UseDocumentation(this WebApplication app)
        {
            if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    var version = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
                    foreach (var description in version.ApiVersionDescriptions)
                    {
                        options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                    }

                });
            }

            return app;
        }
    }

    public class SwaggerConfig : IConfigureOptions<SwaggerGenOptions>
    {
        private readonly IApiVersionDescriptionProvider _apiVersionDescriptionProvider;

        public SwaggerConfig(IApiVersionDescriptionProvider apiVersionDescriptionProvider)
        {
            _apiVersionDescriptionProvider = apiVersionDescriptionProvider;
        }

        public void Configure(SwaggerGenOptions options)
        {
            foreach (var description in _apiVersionDescriptionProvider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(description.GroupName, CreateOpenApiInfo(description));
            }
        }

        private static OpenApiInfo CreateOpenApiInfo(ApiVersionDescription description)
        {
            var info = new OpenApiInfo()
            {
                Title = "Mshop API Cart - Shopping",
                Version = description.ApiVersion.ToString(),
                Description = description.IsDeprecated ? "This API version has been deprecated." : string.Empty,
                //Contact = new OpenApiContact() { Name = "", Email = "", Url = new Uri("") },
                //TermsOfService = new Uri(""),
                //License = new OpenApiLicense() { Name = "", Url = new Uri("") },
            };

            if (description.IsDeprecated)
            {
                info.Description += " - Deprecated (Esta versão esta obsoleta)";
            }
            return info;
        }
    }

    public class SwaggerDefaultValues : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var apiDescription = context.ApiDescription;

            operation.Deprecated |= apiDescription.IsDeprecated();

            if (operation.Parameters == null) return;

            foreach (var parameter in operation.Parameters)
            {
                var apiParameter = apiDescription.ParameterDescriptions
                    .First(p => p.Name == parameter.Name);

                if (apiParameter.RouteInfo != null)
                {
                    parameter.Description = apiParameter.ModelMetadata?.Description;
                }

                if (parameter.Schema.Default is null)
                    parameter.Schema.Default = new OpenApiString(apiParameter.DefaultValue?.ToString() ?? string.Empty);

                parameter.Required |= apiParameter.IsRequired;
            }
        }
    
    }

}
