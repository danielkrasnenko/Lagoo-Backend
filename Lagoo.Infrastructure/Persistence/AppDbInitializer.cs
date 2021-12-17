using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Lagoo.Infrastructure.Persistence;

public static class AppDbInitializer
{
    private const string InMemoryDbName = "Microsoft.EntityFrameworkCore.InMemory";
    
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<AppDbContext>();

        if (context.Database.ProviderName != InMemoryDbName)
        {
            // await context.Database.MigrateAsync();
        }
    }
}