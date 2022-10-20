using Lagoo.BusinessLogic.Common.Mappings;
using Lagoo.Domain.Entities;

namespace Lagoo.BusinessLogic.CommandsAndQueries.Accounts.Common.Dtos;

public class ReadRefreshTokenDto : IMapFrom<RefreshToken>
{
    public long Id { get; set; }
    
    public string Value { get; set; } = string.Empty;
    
    public Guid DeviceId { get; set; }

    public DateTime ExpiresAt { get; set; }

    public DateTime CreatedAt { get; set; }
    
    public DateTime? LastModifiedAt { get; set; }

    public Guid OwnerId { get; set; }
}