namespace Lagoo.BusinessLogic.CommandsAndQueries.Accounts.Commands.LoginUser;

public class LoginUserResponseDto
{
    public string AccessToken { get; set; } = string.Empty;
    
    public DateTime AccessTokenExpiresAt { get; set; }
 
    public string RefreshTokenValue { get; set; } = string.Empty;
    
    public DateTime RefreshTokenExpiresAt { get; set; }
}