namespace Lagoo.BusinessLogic.CommandsAndQueries.Accounts.Commands.RefreshAccessToken;

public class RefreshAccessTokenResponseDto
{
    public string AccessToken { get; set; } = string.Empty;
    
    public DateTime AccessTokenExpiresAt { get; set; }
}