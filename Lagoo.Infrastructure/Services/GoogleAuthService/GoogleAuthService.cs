using Lagoo.BusinessLogic.Common.ExternalServices.GoogleAuthService;
using Lagoo.BusinessLogic.Common.Services.HttpService;

namespace Lagoo.Infrastructure.Services.GoogleAuthService;

/// <summary>
///  A service for authentication via Google
/// </summary>
public class GoogleAuthService : IGoogleAuthService
{
    private const string UserInfoUrl = "https://openidconnect.googleapis.com/v1/userinfo";
    
    private readonly IHttpService _httpService;

    public GoogleAuthService(IHttpService httpService)
    {
        _httpService = httpService;
    }
    
    public Task<GoogleUserInfo> GetUserInfo(string accessToken)
    {
        _httpService.SetBearerToken(accessToken);

        return _httpService.GetAsync<GoogleUserInfo>(UserInfoUrl);
    }
}