using Lagoo.Domain.Extensions;

namespace Lagoo.Domain.Entities;

/// <summary>
///   Refresh Token model for refreshing Access token
/// </summary>
public class RefreshToken
{
    public string Value { get; set; } = string.Empty;
    
    public Guid DeviceId { get; set; }

    public DateTime ExpiresAt
    {
        get => _expiresAtBackingField;
        set => _expiresAtBackingField = value.ConvertToUtc();
    }
    private DateTime _expiresAtBackingField;

    public DateTime? LastModifiedAt
    {
        get => _lastModifiedAtBackingField;
        set => _lastModifiedAtBackingField = value?.ConvertToUtc();
    }
    private DateTime? _lastModifiedAtBackingField;

    public Guid OwnerId { get; set; }
    public AppUser Owner
    {
        get => _ownerBackingField ?? throw new InvalidOperationException($"Uninitialized property: {nameof(Owner)}");
        set => _ownerBackingField = value;
    }
    private AppUser? _ownerBackingField;
}