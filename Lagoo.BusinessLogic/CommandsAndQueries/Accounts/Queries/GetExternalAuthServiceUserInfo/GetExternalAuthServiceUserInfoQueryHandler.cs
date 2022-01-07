using AutoMapper;
using Lagoo.BusinessLogic.Common.Services.ExternalAuthServicesManager;
using MediatR;

namespace Lagoo.BusinessLogic.CommandsAndQueries.Accounts.Queries.GetExternalAuthServiceUserInfo;

/// <summary>
///   Request handler for <see cref="GetExternalAuthServiceUserInfoQuery"/>
/// </summary>
public class GetExternalAuthServiceUserInfoQueryHandler : IRequestHandler<GetExternalAuthServiceUserInfoQuery, GetExternalAuthServiceUserInfoResponseDto>
{
    private readonly IExternalAuthServicesManager _externalAuthServicesManager;

    private readonly IMapper _mapper;

    public GetExternalAuthServiceUserInfoQueryHandler(IExternalAuthServicesManager externalAuthServicesManager, IMapper mapper)
    {
        _externalAuthServicesManager = externalAuthServicesManager;
        _mapper = mapper;
    }
    
    public async Task<GetExternalAuthServiceUserInfoResponseDto> Handle(GetExternalAuthServiceUserInfoQuery request, CancellationToken cancellationToken)
    {
        var userInfo = await _externalAuthServicesManager.GetUserInfoAsync(request.ExternalAuthService,
            request.ExternalAuthServiceAccessToken);

        return _mapper.Map<GetExternalAuthServiceUserInfoResponseDto>(userInfo);
    }
}