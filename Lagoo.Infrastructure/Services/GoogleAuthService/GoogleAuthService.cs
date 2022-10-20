using Lagoo.BusinessLogic.Common.Services.ExternalAuthServicesManager;
using Lagoo.BusinessLogic.Common.Services.GoogleAuthService;
using Lagoo.BusinessLogic.Common.Services.HttpService;

namespace Lagoo.Infrastructure.Services.GoogleAuthService;

/// <summary>
///   A service for authentication via Google
/// </summary>
public class GoogleAuthService : IGoogleAuthService
{
    private const string UserInfoUrl = "https://openidconnect.googleapis.com/v1/userinfo";
    
    private readonly IHttpService _httpService;

    public GoogleAuthService(IHttpService httpService)
    {
        _httpService = httpService;
    }
    
    public async Task<IExternalAuthServiceUserInfo> GetUserInfoAsync(string accessToken)
    {
        _httpService.SetBearerToken(accessToken);

        return await _httpService.GetAsync<GoogleUserInfo>(UserInfoUrl);
    }
}