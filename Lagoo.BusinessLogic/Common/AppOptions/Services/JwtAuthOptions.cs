using Microsoft.IdentityModel.Tokens;

namespace Lagoo.BusinessLogic.Common.AppOptions.Services;

/// <summary>
///  JWT options for Authentication and Authorization
/// </summary>
public class JwtAuthOptions
{
    public const string JwtAuth = "JwtAuth";

    public string Issuer { get; set; } = string.Empty;

    public string Audience { get; set; } = string.Empty;

    public string Secret { get; set; } = string.Empty;
    
    public int AccessTokenExpirationInMin { get; set; }
    
    public int RefreshTokenExpirationInMin { get; set; }

    public static string SecurityAlgorithm => SecurityAlgorithms.HmacSha256Signature;
}