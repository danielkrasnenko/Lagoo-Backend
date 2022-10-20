using AutoMapper;
using AutoMapper.QueryableExtensions;
using Lagoo.BusinessLogic.CommandsAndQueries.Accounts.Common.Dtos;
using Lagoo.BusinessLogic.Core.Repositories;
using Lagoo.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lagoo.Infrastructure.Persistence.Repositories;

public class RefreshTokenRepository : RepositoryBase, IRefreshTokenRepository
{
    private readonly IMapper _mapper;
    
    public RefreshTokenRepository(AppDbContext context, IMapper mapper) : base(context)
    {
        _mapper = mapper;
    }

    public async Task<ReadRefreshTokenDto?> GetAsync(Guid ownerId, Guid deviceId, CancellationToken cancellationToken)
    {
        return await Context.RefreshTokens
            .Where(rt => rt.OwnerId == ownerId && rt.DeviceId == deviceId)
            .ProjectTo<ReadRefreshTokenDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task<ReadRefreshTokenWithOwnerDto?> GetWithOwner(string value, Guid ownerId, CancellationToken cancellationToken)
    {
        return Context.RefreshTokens
            .Where(rt => rt.Value == value && rt.OwnerId == ownerId)
            .Include(rt => rt.Owner)
            .ProjectTo<ReadRefreshTokenWithOwnerDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task<bool> ExistsAsync(Guid ownerId, Guid deviceId, CancellationToken cancellationToken)
    {
        return Context.RefreshTokens
            .Where(rt => rt.OwnerId == ownerId && rt.DeviceId == deviceId)
            .ProjectTo<ReadRefreshTokenDto>(_mapper.ConfigurationProvider)
            .AnyAsync(cancellationToken);
    }

    public async Task<ReadRefreshTokenDto> SaveAsync(RefreshToken refreshToken)
    {
        Context.Add(refreshToken);
        await Context.SaveChangesAsync(CancellationToken.None);

        return _mapper.Map<ReadRefreshTokenDto>(refreshToken);
    }

    public async Task<ReadRefreshTokenDto?> UpdateAsync(Guid ownerId, Guid deviceId,
        UpdateRefreshTokenDto updateRefreshTokenDto, CancellationToken cancellationToken)
    {
        var refreshToken = await Context.RefreshTokens.FirstOrDefaultAsync(rt =>
            rt.OwnerId == ownerId && rt.DeviceId == deviceId, cancellationToken);

        if (refreshToken is null)
        {
            return null;
        }

        refreshToken.Value = updateRefreshTokenDto.Value;
        refreshToken.ExpiresAt = updateRefreshTokenDto.ExpiresAt;
        refreshToken.LastModifiedAt = updateRefreshTokenDto.LastModifiedAt;

        await Context.SaveChangesAsync(CancellationToken.None);

        return _mapper.Map<ReadRefreshTokenDto>(refreshToken);
    }
}