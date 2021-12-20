namespace Lagoo.BusinessLogic.Common.ExternalServices.FacebookAuthService;

/// <summary>
///  An interface for working with Facebook external auth service
/// </summary>
public interface IFacebookAuthService
{
    Task<FacebookUserInfo> GetUserInfo(string accessToken);
}