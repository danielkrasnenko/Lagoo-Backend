namespace Lagoo.Domain.Entities;

/// <summary>
///  Refresh Token
/// </summary>
public class RefreshToken
{
    public string Value { get; set; } = string.Empty;

    public DateTimeOffset ExpirationUtcDate { get; set; }

    public DateTimeOffset? LastModifiedUtcDate { get; set; }

    public AppUser? Owner { get; set; }
    public Guid OwnerId { get; set; }
}