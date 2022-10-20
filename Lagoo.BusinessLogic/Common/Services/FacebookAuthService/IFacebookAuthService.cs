using Lagoo.BusinessLogic.Common.Services.ExternalAuthServicesManager;

namespace Lagoo.BusinessLogic.Common.Services.FacebookAuthService;

/// <summary>
///   An interface for working with Facebook external auth service
/// </summary>
public interface IFacebookAuthService
{
    Task<IExternalAuthServiceUserInfo> GetUserInfoAsync(string accessToken);
}