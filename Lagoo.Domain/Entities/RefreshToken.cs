using Lagoo.Domain.Extensions;

namespace Lagoo.Domain.Entities;

/// <summary>
///   Refresh Token model for refreshing Access token
/// </summary>
public class RefreshToken
{
    public long Id { get; set; }
    
    public string Value { get; set; } = string.Empty;
    
    public Guid DeviceId { get; set; }

    public DateTime ExpiresAt
    {
        get => _expiresAtBackingField;
        set => _expiresAtBackingField = value.ConvertToUtc();
    }
    private DateTime _expiresAtBackingField;

    public DateTime CreatedAt
    {
        get => _createdAtBackingField;
        set => _createdAtBackingField = value.ConvertToUtc();
    }
    private DateTime _createdAtBackingField;
    
    public DateTime? LastModifiedAt
    {
        get => _lastModifiedAtBackingField;
        set => _lastModifiedAtBackingField = value?.ConvertToUtc();
    }
    private DateTime? _lastModifiedAtBackingField;

    public Guid OwnerId { get; set; }
    public AppUser Owner
    {
        get => _owner ?? throw new InvalidOperationException($"Uninitialized property: {nameof(Owner)}");
        set => _owner = value;
    }
    private AppUser? _owner;
}