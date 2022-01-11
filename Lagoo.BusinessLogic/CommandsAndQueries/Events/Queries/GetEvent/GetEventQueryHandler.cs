using AutoMapper;
using AutoMapper.QueryableExtensions;
using Lagoo.BusinessLogic.Common.Exceptions.Api;
using Lagoo.BusinessLogic.Common.ExternalServices.Database;
using Lagoo.BusinessLogic.Resources.CommandsAndQueries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Lagoo.BusinessLogic.CommandsAndQueries.Events.Queries.GetEvent;

public class GetEventQueryHandler : IRequestHandler<GetEventQuery, GetEventResponseDto>
{
    private readonly IAppDbContext _context;

    private readonly IMapper _mapper;

    public GetEventQueryHandler(IAppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<GetEventResponseDto> Handle(GetEventQuery request, CancellationToken cancellationToken)
    {
        var @event = await _context.Events.ProjectTo<GetEventResponseDto>(_mapper.ConfigurationProvider).FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

        return @event ?? throw new NotFoundException(EventResources.EventWasNotFound);
    }
}