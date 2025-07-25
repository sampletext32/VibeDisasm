using Microsoft.AspNetCore.Cors.Infrastructure;

namespace VibeDisasm.Web.Extensions;

public static class CorsExtensions
{
    public static IServiceCollection AddCorsConfiguration(
        this IServiceCollection services,
        IConfiguration configuration,
        ILogger? logger = null
    )
    {
        var hosts = configuration.GetSection("Cors:AllowedOrigins").Get<string[]?>() ?? [];

        logger?.LogInformation("Cors configuration contains {AllowedOrigins}", string.Join(',', hosts));

        services.AddCors()
            .Configure<CorsOptions>(options =>
                {
                    options.AddPolicy(
                        "CorsPolicy",
                        p =>
                        {
                            p.AllowAnyHeader()
                                .AllowAnyMethod()
                                .AllowCredentials()
                                .WithOrigins(hosts)
                                .SetIsOriginAllowedToAllowWildcardSubdomains();
                        }
                    );
                }
            );

        return services;
    }

    public static IApplicationBuilder UseCorsConfiguration(this IApplicationBuilder app) => app.UseCors("CorsPolicy");
}
