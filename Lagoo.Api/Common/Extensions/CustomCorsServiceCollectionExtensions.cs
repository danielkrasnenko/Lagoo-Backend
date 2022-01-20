namespace Lagoo.Api.Common.Extensions;

/// <summary>
///   Custom CORS extension methods for <see cref="IServiceCollection"/>
/// </summary>
public static class CustomCorsServiceCollectionExtensions
{
    public const string CorsPolicyName = "EnableCORS";
    
    public static void AddCustomCors(this IServiceCollection services, IWebHostEnvironment environment)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(CorsPolicyName, builder =>
            {
                builder.WithOrigins(environment.IsDevelopment()
                    ? "http://localhost:4200"
                    : "https://www.lagoo.com/");

                builder
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
                    .WithExposedHeaders("Content-Disposition");
            });
        });
    }
}