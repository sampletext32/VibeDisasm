using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using VibeDisasm.Web.Utils;
using Path = System.IO.Path;

namespace VibeDisasm.Web.Extensions;

internal static class ServiceCollectionExtensions
{
    internal static IServiceCollection MyConfigureSwagger(this IServiceCollection services)
    {
        services.Configure<SwaggerGenOptions>(
            options =>
            {
                options.CustomSchemaIds(SwaggerTypeNamesProvider.GetSwaggerDisplayedName);
                options.CustomOperationIds(SwaggerOperationIdProvider.GenSwaggerOperationId);

                options.DescribeAllParametersInCamelCase();

                options.IncludeXmlComments(
                    Path.Combine(
                        AppContext.BaseDirectory,
                        "VibeDisasm.Web.xml"
                    )
                );

                options.SwaggerDoc(
                    "v1",
                    new OpenApiInfo
                    {
                        Version = "v1",
                        Title = "VibeDisasm API",
                        Description = "VibeDisasm API"
                    }
                );

                // Add /api prefix to all operations
                // options.AddServer(new OpenApiServer
                // {
                //     Url = "/api",
                //     Description = "API with /api prefix"
                // });
                options.MapType<TimeSpan>(
                    () => new OpenApiSchema
                    {
                        Type = "string",
                        Example = new OpenApiString("00:00")
                    }
                );
            }
        );

        services.Configure<SwaggerUIOptions>(
            options =>
            {
                options.ConfigObject.TryItOutEnabled = true;
                options.RoutePrefix = "swagger";
                options.SwaggerEndpoint("v1/swagger.json", "VibeDisasm API V1");
            }
        );

        return services;
    }
}
