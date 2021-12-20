using Lagoo.BusinessLogic.Common.ExternalServices.FacebookAuthService;
using Lagoo.BusinessLogic.Common.Services.HttpService;

namespace Lagoo.Infrastructure.Services.FacebookAuthService;

/// <summary>
///  A service for authentication via Facebook
/// </summary>
public class FacebookAuthService : IFacebookAuthService
{
    private const string UserInfoUrl = "https://graph.facebook.com/v9.0/me?fields=id,first_name,last_name,email";
    
    private readonly IHttpService _httpService;

    public FacebookAuthService(IHttpService httpService)
    {
        _httpService = httpService;
    }
    
    public Task<FacebookUserInfo> GetUserInfo(string accessToken)
    {
        _httpService.SetBearerToken(accessToken);

        return _httpService.GetAsync<FacebookUserInfo>(UserInfoUrl);
    }
}