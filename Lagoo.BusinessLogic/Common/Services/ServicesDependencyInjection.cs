using Lagoo.BusinessLogic.Common.Services.HttpService;
using Lagoo.BusinessLogic.Common.Services.JwtAuthService;
using Microsoft.Extensions.DependencyInjection;

namespace Lagoo.BusinessLogic.Common.Services;

/// <summary>
///  Adding business logic services into DI container
/// </summary>
public static class ServicesDependencyInjection
{
    public static void AddBusinessLogicServices(this IServiceCollection services)
    {
        services.AddHttpClient<IHttpService, HttpService.HttpService>();

        services.AddScoped<IJwtAuthService, JwtAuthService.JwtAuthService>();
    }
}