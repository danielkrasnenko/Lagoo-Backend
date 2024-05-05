using Lagoo.Domain.Entities;
using Lagoo.Domain.Enums;
using Lagoo.Infrastructure.AppOptions;
using Lagoo.Infrastructure.AppOptions.Databases;
using Lagoo.Infrastructure.Persistence;
using Lagoo.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Lagoo.Infrastructure;

/// <summary>
///   Adding all infrastructure services to DI Container
/// </summary>
public static class DependencyInjection
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAppOptions(configuration);
        
        var dataSourceBuilder = new NpgsqlDataSourceBuilder(BuildDbConnectionString(configuration));

        dataSourceBuilder.EnableDynamicJson();
        dataSourceBuilder.MapEnums();

        var dataSource = dataSourceBuilder.Build();

        services.AddDbContext<AppDbContext>(options =>
                options
                    .UseNpgsql(dataSource,
                        builder =>
                        {
                            builder.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName);
                            builder.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery);
                        })
                    .UseSnakeCaseNamingConvention()
            )
            .AddHealthChecks()
            .AddDbContextCheck<AppDbContext>();

        services.AddAspIdentity();
        
        services.AddInfrastructureRepositories();

        services.AddInfrastructureServices();
    }

    private static void MapEnums(this NpgsqlDataSourceBuilder dataSourceBuilder)
    {
        dataSourceBuilder.MapEnum<EventType>();
    }

    private static void AddAspIdentity(this IServiceCollection services)
    {
        services
            .AddIdentity<AppUser, IdentityRole<Guid>>(options => options.User.RequireUniqueEmail = true)
            .AddEntityFrameworkStores<AppDbContext>()
            .AddUserStore<UserStore<AppUser, IdentityRole<Guid>, AppDbContext, Guid, IdentityUserClaim<Guid>,
                IdentityUserRole<Guid>, IdentityUserLogin<Guid>, IdentityUserToken<Guid>, IdentityRoleClaim<Guid>>>()
            .AddRoleStore<RoleStore<IdentityRole<Guid>, AppDbContext, Guid, IdentityUserRole<Guid>,
                IdentityRoleClaim<Guid>>>()
            .AddDefaultTokenProviders();
    }

    private static string BuildDbConnectionString(IConfiguration configuration)
    {
        var postgresStringBuilder = new NpgsqlConnectionStringBuilder(
            configuration.GetConnectionString(MainDatabaseOptions.MainDatabaseConnection));

        return postgresStringBuilder.ConnectionString;
    }
}