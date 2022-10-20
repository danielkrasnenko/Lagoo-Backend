using Lagoo.BusinessLogic.Core.Repositories;

namespace Lagoo.Infrastructure.Persistence.Repositories;

public class RepositoryBase : IRepository
{
    protected readonly AppDbContext Context;

    public RepositoryBase(AppDbContext context)
    {
        Context = context;
    }

    public async ValueTask DisposeAsync()
    {
        await Context.DisposeAsync();
        GC.SuppressFinalize(this);
    }
}