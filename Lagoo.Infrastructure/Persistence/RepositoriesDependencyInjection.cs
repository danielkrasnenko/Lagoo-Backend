using Lagoo.BusinessLogic.Core.Repositories;
using Lagoo.Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Lagoo.Infrastructure.Persistence;

public static class RepositoriesDependencyInjection
{
    public static void AddInfrastructureRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();

        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        
        services.AddScoped<IEventRepository, EventRepository>();
    }
}