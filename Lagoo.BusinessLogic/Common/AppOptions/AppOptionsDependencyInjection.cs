using Lagoo.BusinessLogic.Common.AppOptions.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Lagoo.BusinessLogic.Common.AppOptions;

/// <summary>
///  Adding IOptions from appsettings to DI Container
/// </summary>
public static class AppOptionsDependencyInjection
{
    public static void AddAppOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions();

        services.Configure<JwtAuthOptions>(configuration.GetSection(JwtAuthOptions.JwtAuth));
        
        services.PopulateConfiguredOptionsWithUserSecretsData(configuration);
    }

    private static void PopulateConfiguredOptionsWithUserSecretsData(this IServiceCollection services, IConfiguration configuration)
    {
        services.PostConfigure<JwtAuthOptions>(options =>
        {
            options.Secret = configuration[UserSecrets.UserSecrets.JwtSecret];
        });
    }
}