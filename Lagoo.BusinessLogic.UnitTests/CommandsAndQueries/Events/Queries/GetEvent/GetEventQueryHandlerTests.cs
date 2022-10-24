using System;
using System.Threading;
using System.Threading.Tasks;
using Lagoo.BusinessLogic.CommandsAndQueries.Events.Common.Dtos;
using Lagoo.BusinessLogic.CommandsAndQueries.Events.Queries.GetEvent;
using Lagoo.BusinessLogic.Common.Exceptions.Api;
using Lagoo.BusinessLogic.UnitTests.Common.Base;
using Lagoo.Domain.Enums;
using NSubstitute;
using NUnit.Framework;

namespace Lagoo.BusinessLogic.UnitTests.CommandsAndQueries.Events.Queries.GetEvent;

/// <summary>
///   Tests for <see cref="GetEventQueryHandler"/>
/// </summary>
[TestFixture]
public class GetEventQueryHandlerTests : TestsBase
{
    private const long DefaultEventId = 1;

    private readonly ReadEventDto _defaultReadEventDto;

    public GetEventQueryHandlerTests()
    {
        _defaultReadEventDto = CreateDefaultReadEventDto();
        EventRepository.GetAsync(DefaultEventId, CancellationToken.None).ReturnsForAnyArgs(_defaultReadEventDto);
    }
    
    [Test]
    public async Task Handle_EventExists_ShouldReturnEventWithSpecifiedId()
    {
        var query = new GetEventQuery
        {
            EventId = DefaultEventId
        };

        var handler = CreateHandler();
        
        var result = await handler.Handle(query, CancellationToken.None);

        Assert.AreEqual(DefaultEventId, result.Id);
        Assert.AreEqual(_defaultReadEventDto.Name, result.Name);
        Assert.AreEqual(_defaultReadEventDto.Type, result.Type);
        Assert.AreEqual(_defaultReadEventDto.Address, result.Address);
        Assert.AreEqual(_defaultReadEventDto.Comment, result.Comment);
        Assert.AreEqual(_defaultReadEventDto.Duration, result.Duration);
        Assert.AreEqual(_defaultReadEventDto.IsPrivate, result.IsPrivate);
        Assert.AreEqual(_defaultReadEventDto.BeginsAt, result.BeginsAt);
        Assert.AreEqual(_defaultReadEventDto.CreatedAt, result.CreatedAt);

        if (_defaultReadEventDto.LastModifiedAt is null)
        {
            Assert.Null(result.LastModifiedAt);
        }
        else
        {
            Assert.NotNull(result.LastModifiedAt);
            Assert.AreEqual(_defaultReadEventDto.LastModifiedAt, result.LastModifiedAt ?? default);
        }
    }
    
    [Test]
    public void Handle_EventDoesNotExist_ThrowsNotFoundException()
    {
        var query = new GetEventQuery
        {
            EventId = long.MaxValue
        };

        var handler = CreateHandler();
        
        Assert.ThrowsAsync<NotFoundException>( () => handler.Handle(query, CancellationToken.None));
    }

    private GetEventQueryHandler CreateHandler() => new (EventRepository);

    private ReadEventDto CreateDefaultReadEventDto() => new()
    {
        Id = DefaultEventId,
        Name = "Last Night",
        Type = EventType.Party,
        Address = "Down street '60",
        Comment = "It's gonna be nice",
        Duration = TimeSpan.FromHours(3),
        IsPrivate = true,
        BeginsAt = DateTime.UtcNow,
        CreatedAt = DateTime.UtcNow.AddDays(-4)
    };
}