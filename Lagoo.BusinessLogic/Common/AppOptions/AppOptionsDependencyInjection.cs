using Lagoo.BusinessLogic.Common.AppOptions.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Lagoo.BusinessLogic.Common.AppOptions;

/// <summary>
///  Adding <see cref="IOptions{TOptions}"/> configs from appsettings to DI Container
/// </summary>
public static class AppOptionsDependencyInjection
{
    public static void AddAppOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions();

        services.Configure<JwtAuthOptions>(configuration.GetSection(JwtAuthOptions.JwtAuth));
    }
}