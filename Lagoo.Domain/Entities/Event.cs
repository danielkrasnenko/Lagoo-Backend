using Lagoo.Domain.Enums;
using Lagoo.Domain.Extensions;

namespace Lagoo.Domain.Entities;

/// <summary>
///   A model for representing events
/// </summary>
public class Event
{
    public long Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public EventType Type { get; set; }

    public string Address { get; set; } = string.Empty;

    public string Comment { get; set; } = string.Empty;

    public bool IsPrivate { get; set; }

    public TimeSpan Duration { get; set; }

    public DateTime BeginsAt
    {
        get => _beginsAtBackingField;
        set => _beginsAtBackingField = value.ConvertToUtc();
    }
    private DateTime _beginsAtBackingField;

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
}