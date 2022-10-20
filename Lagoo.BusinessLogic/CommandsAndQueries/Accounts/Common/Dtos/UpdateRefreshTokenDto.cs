namespace Lagoo.BusinessLogic.CommandsAndQueries.Accounts.Common.Dtos;

public class UpdateRefreshTokenDto
{
    public string Value { get; set; } = string.Empty;
    
    public DateTime ExpiresAt { get; set; }

    public DateTime? LastModifiedAt { get; set; }
}