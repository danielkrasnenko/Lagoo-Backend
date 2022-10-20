using Lagoo.BusinessLogic.Core.Repositories;
using Lagoo.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Lagoo.Infrastructure.Persistence.Repositories;

public class UserRepository : RepositoryBase, IUserRepository
{
    private readonly UserManager<AppUser> _userManager;
    
    public UserRepository(AppDbContext context, UserManager<AppUser> userManager) : base(context)
    {
        _userManager = userManager;
    }

    public Task<AppUser> FindByEmailAsync(string email)
    {
        return _userManager.FindByEmailAsync(email);
    }

    public Task<AppUser> FindByLoginAsync(string loginProvider, string providerKey)
    {
        return _userManager.FindByLoginAsync(loginProvider, providerKey);
    }

    public Task<IdentityResult> CreateAsync(AppUser user)
    {
        return _userManager.CreateAsync(user);
    }

    public Task<IdentityResult> CreateAsync(AppUser user, string password)
    {
        return _userManager.CreateAsync(user, password);
    }

    public Task<bool> CheckPasswordAsync(AppUser user, string password)
    {
        return _userManager.CheckPasswordAsync(user, password);
    }

    public Task<IdentityResult> AddToRoleAsync(AppUser user, string role)
    {
        return _userManager.AddToRoleAsync(user, role);
    }
}