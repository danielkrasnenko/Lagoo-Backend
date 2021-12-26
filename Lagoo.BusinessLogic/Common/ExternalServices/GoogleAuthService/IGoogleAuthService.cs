namespace Lagoo.BusinessLogic.Common.ExternalServices.GoogleAuthService;

/// <summary>
///  An interface for working with Google external auth service
/// </summary>
public interface IGoogleAuthService
{
    Task<GoogleUserInfo> GetUserInfoAsync(string accessToken);
}