using Lagoo.Domain.Entities;
using Lagoo.Infrastructure.AppOptions;
using Lagoo.Infrastructure.AppOptions.Databases;
using Lagoo.Infrastructure.Persistence;
using Lagoo.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Lagoo.Infrastructure;

/// <summary>
///   Adding all infrastructure services to DI Container
/// </summary>
public static class DependencyInjection
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAppOptions(configuration);
        
        services.AddDbContext<AppDbContext>(options => options.UseSqlServer(BuildDbConnectionString(configuration),
            builder =>
            {
                builder.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName);
                builder.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery);
            }));

        services.AddAspIdentity();
        
        services.AddInfrastructureRepositories();

        services.AddInfrastructureServices();
    }

    private static void AddAspIdentity(this IServiceCollection services)
    {
        services
            .AddIdentity<AppUser, IdentityRole<Guid>>(options => options.User.RequireUniqueEmail = true)
            .AddEntityFrameworkStores<AppDbContext>()
            .AddUserStore<UserStore<AppUser, IdentityRole<Guid>, AppDbContext, Guid>>()
            .AddRoleStore<RoleStore<IdentityRole<Guid>, AppDbContext, Guid>>()
            .AddDefaultTokenProviders();
    }

    private static string BuildDbConnectionString(IConfiguration configuration)
    {
        var sqlConStrBuilder = new SqlConnectionStringBuilder(configuration.GetConnectionString(MainDatabaseOptions.MainDatabaseConnection))
        {
            Password = configuration[UserSecrets.UserSecrets.MainDatabasePassword]
        };

        return sqlConStrBuilder.ConnectionString;
    }
}