using Lagoo.BusinessLogic.Common.Services.ExternalAuthServicesManager;

namespace Lagoo.BusinessLogic.Common.Services.GoogleAuthService;

/// <summary>
///   An interface for working with Google external auth service
/// </summary>
public interface IGoogleAuthService
{
    Task<IExternalAuthServiceUserInfo> GetUserInfoAsync(string accessToken);
}