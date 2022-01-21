using System;
using System.Threading;
using System.Threading.Tasks;
using Lagoo.BusinessLogic.CommandsAndQueries.Events.Queries.GetEvent;
using Lagoo.BusinessLogic.Common.Exceptions.Api;
using Lagoo.BusinessLogic.UnitTests.Common.Base;
using Lagoo.BusinessLogic.UnitTests.Common.Helpers;
using Lagoo.Domain.Entities;
using Lagoo.Domain.Enums;
using NUnit.Framework;

namespace Lagoo.BusinessLogic.UnitTests.CommandsAndQueries.Events.Queries.GetEvent;

[TestFixture]
public class GetEventQueryHandlerTests : TestBase
{
    private const long DefaultEventId = 1;

    private readonly Event _defaultEvent;

    public GetEventQueryHandlerTests()
    {
        _defaultEvent = CreateDefaultEvent();
        Context.Events = TestHelpers.MockDbSet(_defaultEvent);
    }
    
    [Test]
    public async Task Handle_EventExists_ShouldReturnEventWithSpecifiedId()
    {
        var query = new GetEventQuery
        {
            Id = DefaultEventId
        };

        var handler = CreateHandler();
        
        var result = await handler.Handle(query, CancellationToken.None);

        Assert.AreEqual(DefaultEventId, result.Id);
        Assert.AreEqual(_defaultEvent.Name, result.Name);
        Assert.AreEqual(_defaultEvent.Type, result.Type);
        Assert.AreEqual(_defaultEvent.Address, result.Address);
        Assert.AreEqual(_defaultEvent.Comment, result.Comment);
        Assert.AreEqual(_defaultEvent.Duration, result.Duration);
        Assert.AreEqual(_defaultEvent.IsPrivate, result.IsPrivate);
        Assert.AreEqual(_defaultEvent.BeginsAt, result.BeginsAt);
        Assert.AreEqual(_defaultEvent.CreatedAt, result.CreatedAt);

        if (_defaultEvent.LastModifiedAt is null)
        {
            Assert.Null(result.LastModifiedAt);
        }
        else
        {
            Assert.NotNull(result.LastModifiedAt);
            Assert.AreEqual(_defaultEvent.LastModifiedAt, result.LastModifiedAt ?? default);
        }
    }
    
    [Test]
    public void Handle_EventDoesNotExist_ThrowsNotFoundException()
    {
        var query = new GetEventQuery
        {
            Id = long.MaxValue
        };

        var handler = CreateHandler();
        
        Assert.ThrowsAsync<NotFoundException>( () => handler.Handle(query, CancellationToken.None));
    }

    private GetEventQueryHandler CreateHandler() => new (Context, Mapper);

    private Event CreateDefaultEvent() => new()
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