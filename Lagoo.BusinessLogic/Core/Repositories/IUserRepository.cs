using Lagoo.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Lagoo.BusinessLogic.Core.Repositories;

public interface IUserRepository : IRepository
{
    Task<AppUser> FindByEmailAsync(string email);
    
    Task<AppUser> FindByLoginAsync(string loginProvider, string providerKey);

    Task<IdentityResult> CreateAsync(AppUser user);
    
    Task<IdentityResult> CreateAsync(AppUser user, string password);

    Task<bool> CheckPasswordAsync(AppUser user, string password);

    Task<IdentityResult> AddToRoleAsync(AppUser user, string role);
}