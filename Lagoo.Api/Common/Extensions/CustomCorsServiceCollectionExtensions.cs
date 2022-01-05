namespace Lagoo.Api.Common.Extensions;

/// <summary>
///   Custom CORS extension methods for <see cref="IServiceCollection"/>
/// </summary>
public static class CustomCorsServiceCollectionExtensions
{
    public static void AddCustomCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("EnableCORS", builder =>
            {
                builder
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .WithExposedHeaders("Content-Disposition");
            });
        });
    }
}