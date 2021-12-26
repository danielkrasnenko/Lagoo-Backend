namespace Lagoo.BusinessLogic.CommandsAndQueries.Accounts.Commands.RegisterUser;

public class RegisterUserResponseDto
{
    public string AccessToken { get; set; } = string.Empty;
    
    public DateTime AccessTokenExpiresAt { get; set; }
 
    public string RefreshTokenValue { get; set; } = string.Empty;
    
    public DateTime RefreshTokenExpiresAt { get; set; }
}