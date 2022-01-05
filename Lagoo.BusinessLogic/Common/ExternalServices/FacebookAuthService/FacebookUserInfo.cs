using System.Text.Json.Serialization;
using Lagoo.BusinessLogic.Common.ExternalServices.Models;

namespace Lagoo.BusinessLogic.Common.ExternalServices.FacebookAuthService;

/// <summary>
///   User information from Facebook
/// </summary>
public class FacebookUserInfo : IExternalAuthServiceUserInfo
{
    public string Id { get; set; } = string.Empty;
    
    [JsonPropertyName("first_name")]
    public string FirstName { get; set; } = string.Empty;
    
    [JsonPropertyName("last_name")]
    public string LastName { get; set; } = string.Empty;
    
    public string Email { get; set; } = string.Empty;
    
    public override string ToString() => $"{FirstName} {LastName}";
}