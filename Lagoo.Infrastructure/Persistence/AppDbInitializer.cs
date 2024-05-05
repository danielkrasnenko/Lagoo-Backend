using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Lagoo.Infrastructure.Persistence;

public class AppDbInitializer
{
    private readonly WebApplication _application;
    
    public AppDbInitializer(WebApplication application)
    {
        _application = application;
    }
    
    public async Task InitializeAsync()
    {
        using var scope = _application.Services.CreateScope();

        try
        {
            var context = scope.ServiceProvider.GetService<AppDbContext>();

            if (context is not null && context.Database.IsNpgsql())
            {
                await SyncDatabaseAsync(context);
            }
        }
        catch (Exception exception)
        {
            var logger = scope.ServiceProvider.GetService<ILogger<AppDbInitializer>>();
            logger?.LogError(exception, "An error occurred while seeding the database");
        }
    }

    private Task SyncDatabaseAsync(AppDbContext context)
    {
        return context.Database.MigrateAsync();
    }
}