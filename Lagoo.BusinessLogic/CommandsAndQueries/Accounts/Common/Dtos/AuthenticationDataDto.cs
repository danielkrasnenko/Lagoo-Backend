namespace Lagoo.BusinessLogic.CommandsAndQueries.Accounts.Common.Dtos;

public class AuthenticationDataDto
{
    public string AccessToken { get; set; } = string.Empty;
    
    public DateTime AccessTokenExpiresAt { get; set; }
 
    public string RefreshTokenValue { get; set; } = string.Empty;
    
    public DateTime RefreshTokenExpiresAt { get; set; }

    public Guid DeviceId { get; set; }
}