using Lagoo.BusinessLogic.Common.Services.ExternalAuthServicesManager;
using Lagoo.BusinessLogic.Common.Services.FacebookAuthService;
using Lagoo.BusinessLogic.Common.Services.HttpService;

namespace Lagoo.Infrastructure.Services.FacebookAuthService;

/// <summary>
///   A service for authentication via Facebook
/// </summary>
public class FacebookAuthService : IFacebookAuthService
{
    private const string UserInfoUrl = "https://graph.facebook.com/v9.0/me?fields=id,first_name,last_name,email";
    
    private readonly IHttpService _httpService;

    public FacebookAuthService(IHttpService httpService)
    {
        _httpService = httpService;
    }
    
    public async Task<IExternalAuthServiceUserInfo> GetUserInfoAsync(string accessToken)
    {
        _httpService.SetBearerToken(accessToken);

        return await _httpService.GetAsync<FacebookUserInfo>(UserInfoUrl);
    }
}