using Lagoo.BusinessLogic.Common.ExternalServices.Database;
using Lagoo.Domain.Entities;
using Lagoo.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Lagoo.Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddContext(configuration);

        services.AddAspIdentity();
        
        services.AddScoped<IAppDbContext>(provider => provider.GetService<AppDbContext>() ?? throw new InvalidOperationException("DB context is not provided."));
    }

    private static void AddContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
        {
            var sqlConStrBuilder = new SqlConnectionStringBuilder(configuration.GetConnectionString("DefaultConnection"))
            {
                Password = configuration["DbPassword"]
            };

            options.UseSqlServer(
                sqlConStrBuilder.ConnectionString,
                builder => builder.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)
            );
        });
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
}