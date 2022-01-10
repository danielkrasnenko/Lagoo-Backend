using Lagoo.BusinessLogic.Common.Exceptions.Api;
using Lagoo.BusinessLogic.Common.Extensions;
using Lagoo.BusinessLogic.Common.ExternalServices.FacebookAuthService;
using Lagoo.BusinessLogic.Common.ExternalServices.GoogleAuthService;
using Lagoo.BusinessLogic.Common.ExternalServices.Models;
using Lagoo.BusinessLogic.Resources.CommandsAndQueries;
using Lagoo.Domain.Entities;
using Lagoo.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;

namespace Lagoo.BusinessLogic.Common.Services.ExternalAuthServicesManager;

public class ExternalAuthServicesManager : IExternalAuthServicesManager
{
    private readonly UserManager<AppUser> _userManager;
    
    private readonly IFacebookAuthService _facebookAuthService;

    private readonly IGoogleAuthService _googleAuthService;

    private readonly IStringLocalizer<AccountResources> _accountLocalizer;

    public ExternalAuthServicesManager(UserManager<AppUser> userManager, IFacebookAuthService facebookAuthService, IGoogleAuthService googleAuthService, IStringLocalizer<AccountResources> accountLocalizer)
    {
        _userManager = userManager;
        _facebookAuthService = facebookAuthService;
        _googleAuthService = googleAuthService;
        _accountLocalizer = accountLocalizer;
    }
    
    public async Task<IExternalAuthServiceUserInfo> GetUserInfoAsync(ExternalAuthService externalAuthService, string accessToken) => externalAuthService switch
    {
        ExternalAuthService.Facebook => await _facebookAuthService.GetUserInfoAsync(accessToken),
        ExternalAuthService.Google => await _googleAuthService.GetUserInfoAsync(accessToken),
        _ => throw new ArgumentOutOfRangeException(nameof(externalAuthService), _accountLocalizer["InvalidExternalAuthService"])
    };

    public async Task<IdentityResult> BindUserAsync(AppUser user, ExternalAuthService externalAuthService, string accessToken)
    {
        var userInfo = await GetUserInfoAsync(externalAuthService, accessToken);

        return await _userManager.AddLoginAsync(user, new UserLoginInfo(externalAuthService.GetEnumDescription(), userInfo.Id, userInfo.ToString()));
    }

    public async Task<IdentityResult> UnbindUserAsync(AppUser user, ExternalAuthService externalAuthService)
    {
        var logins = await _userManager.GetLoginsAsync(user);

        if (logins is null || !logins.Any())
        {
            throw new BadRequestException(_accountLocalizer["UserDoesNotHaveExternalLogins"]);
        }

        var loginProvider = externalAuthService.GetEnumDescription();
        var userLoginInfo = logins.SingleOrDefault(uli => uli.LoginProvider == loginProvider);
        
        if (userLoginInfo is null)
        {
            var res = AccountResources.PasswordIsTooShort;
            throw new BadRequestException(_accountLocalizer["UserDoesNotHaveSpecificExternalLogin"]);
        }

        return await _userManager.RemoveLoginAsync(user, userLoginInfo.LoginProvider, userLoginInfo.ProviderKey);
    }
}