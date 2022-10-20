using Lagoo.BusinessLogic.Common.Services.ExternalAuthServicesManager;
using Lagoo.BusinessLogic.Common.Services.FacebookAuthService;
using Lagoo.BusinessLogic.Common.Services.GoogleAuthService;
using Lagoo.BusinessLogic.Common.Services.HttpService;
using Lagoo.BusinessLogic.Common.Services.JwtAuthService;
using Lagoo.BusinessLogic.Common.Services.UserAccessor;
using Lagoo.BusinessLogic.Common.UserAccessor;
using Microsoft.Extensions.DependencyInjection;

namespace Lagoo.Infrastructure.Services;

public static class ServicesDependencyInjection
{
    public static void AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<IFacebookAuthService, FacebookAuthService.FacebookAuthService>();

        services.AddScoped<IGoogleAuthService, GoogleAuthService.GoogleAuthService>();
        
        services.AddScoped<IExternalAuthServicesManager, ExternalAuthServicesManager.ExternalAuthServicesManager>();
        
        services.AddHttpClient<IHttpService, HttpService>();

        services.AddScoped<IJwtAuthService, JwtAuthService.JwtAuthService>();
        
        services.AddScoped<IUserAccessor, UserAccessor>();
    }
}