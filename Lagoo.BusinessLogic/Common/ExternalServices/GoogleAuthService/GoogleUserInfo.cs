using System.Text.Json.Serialization;
using Lagoo.BusinessLogic.Common.ExternalServices.Models;

namespace Lagoo.BusinessLogic.Common.ExternalServices.GoogleAuthService;

/// <summary>
///  User information from Google
/// </summary>
public class GoogleUserInfo : IExternalAuthServiceUserInfo
{
    [JsonPropertyName("sub")]
    public string Id { get; set; } = string.Empty;
    
    [JsonPropertyName("given_name")]
    public string FirstName { get; set; } = string.Empty;
    
    [JsonPropertyName("family_name")]
    public string LastName { get; set; } = string.Empty;
    
    public string Email { get; set; } = string.Empty;
    
    public override string ToString() => $"{FirstName} {LastName}";
}