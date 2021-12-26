using Lagoo.BusinessLogic.Common.ExternalServices.Models;
using Lagoo.Domain.Entities;
using Lagoo.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace Lagoo.BusinessLogic.Common.Services.ExternalAuthServicesManager;

public interface IExternalAuthServicesManager
{
    Task<IExternalAuthServiceUserInfo> GetUserInfoAsync(ExternalAuthService externalAuthService, string accessToken);

    Task<IdentityResult> BindUserAsync(AppUser user, ExternalAuthService externalAuthService, string accessToken);

    Task<IdentityResult> UnbindUserAsync(AppUser user, ExternalAuthService externalAuthService);
}