using Lagoo.BusinessLogic.Common.ExternalServices.FacebookAuthService;
using Lagoo.BusinessLogic.Common.ExternalServices.GoogleAuthService;
using Microsoft.Extensions.DependencyInjection;

namespace Lagoo.Infrastructure.Services;

public static class ServicesDependencyInjection
{
    public static void AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<IFacebookAuthService, FacebookAuthService.FacebookAuthService>();

        services.AddScoped<IGoogleAuthService, GoogleAuthService.GoogleAuthService>();
    }
}