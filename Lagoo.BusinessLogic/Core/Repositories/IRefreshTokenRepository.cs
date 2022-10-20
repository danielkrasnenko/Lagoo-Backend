using Lagoo.BusinessLogic.CommandsAndQueries.Accounts.Common.Dtos;
using Lagoo.Domain.Entities;

namespace Lagoo.BusinessLogic.Core.Repositories;

public interface IRefreshTokenRepository : IRepository
{
    Task<ReadRefreshTokenDto?> GetAsync(Guid ownerId, Guid deviceId, CancellationToken cancellationToken);

    Task<ReadRefreshTokenWithOwnerDto?> GetWithOwner(string value, Guid ownerId, CancellationToken cancellationToken);

    Task<bool> ExistsAsync(Guid ownerId, Guid deviceId, CancellationToken cancellationToken);

    Task<ReadRefreshTokenDto> SaveAsync(RefreshToken refreshToken);

    Task<ReadRefreshTokenDto?> UpdateAsync(Guid ownerId, Guid deviceId, UpdateRefreshTokenDto updateRefreshTokenDto, CancellationToken cancellationToken);
}