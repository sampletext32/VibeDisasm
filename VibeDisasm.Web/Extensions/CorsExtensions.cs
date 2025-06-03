namespace VibeDisasm.Web.Extensions;

public static class CorsExtensions
{
    public static IServiceCollection AddCorsConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var hosts = configuration.GetSection("Cors:AllowedOrigins").Get<string[]?>() ?? [];

        Console.WriteLine("using hosts: " + string.Join(", ", hosts));

        return services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", p =>
            {
                p.AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()
                    .WithOrigins(hosts)
                    .SetIsOriginAllowedToAllowWildcardSubdomains();
            });
        });
    }

    public static IApplicationBuilder UseCorsConfiguration(this IApplicationBuilder app)
    {
        return app.UseCors("CorsPolicy");
    }
}
