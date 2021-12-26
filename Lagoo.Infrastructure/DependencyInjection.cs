using Lagoo.BusinessLogic.Common.AppOptions.Databases;
using Lagoo.BusinessLogic.Common.ExternalServices.Database;
using Lagoo.BusinessLogic.Common.UserSecrets;
using Lagoo.Domain.Entities;
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
///  Adding all infrastructure services to DI Container
/// </summary>
public static class DependencyInjection
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options => options.UseSqlServer(BuildDbConnectionString(configuration),
            builder => builder.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

        services.AddAspIdentity();
        
        services.AddScoped<IAppDbContext>(provider => provider.GetService<AppDbContext>() ?? throw new InvalidOperationException("DB context is not provided."));

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
            Password = configuration[UserSecrets.MainDatabasePassword]
        };

        return sqlConStrBuilder.ConnectionString;
    }
}