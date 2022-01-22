using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lagoo.BusinessLogic.CommandsAndQueries.Events.Queries.GetEvents;
using Lagoo.BusinessLogic.Common.Enums;
using Lagoo.BusinessLogic.Common.Exceptions.Api;
using Lagoo.BusinessLogic.UnitTests.Common.Base;
using Lagoo.BusinessLogic.UnitTests.Common.Helpers;
using Lagoo.Domain.Entities;
using Lagoo.Domain.Enums;
using NUnit.Framework;

namespace Lagoo.BusinessLogic.UnitTests.CommandsAndQueries.Events.Queries.GetEvents;

[TestFixture]
public class GetEventsQueryHandlerTests : TestBase
{
    [Test]
    public async Task Handle_ThereAreNoEventsInDatabase_ShouldReturnEmptyList()
    {
        Context.Events = TestHelpers.MockDbSet(Array.Empty<Event>());

        var query = new GetEventsQuery();

        var handler = CreateHandler();

        var result = await handler.Handle(query, CancellationToken.None);
        
        Assert.IsEmpty(result.Events);
        Assert.AreEqual(0, result.Count);
    }

    [Test]
    public async Task Handle_ThereAreEventsInDatabase_ShouldReturnListWithEvents()
    {
        Context.Events = TestHelpers.MockDbSet(CreateBasicEvent(1), CreateBasicEvent(2));

        var query = new GetEventsQuery();

        var handler = CreateHandler();

        var result = await handler.Handle(query, CancellationToken.None);
        
        Assert.IsNotEmpty(result.Events);
        Assert.AreNotEqual(0, result.Count);
    }

    [TestCase(EventType.Ceremony)]
    [TestCase(EventType.Convention)]
    [TestCase(EventType.Festival)]
    [TestCase(EventType.Happening)]
    [TestCase(EventType.Party)]
    [TestCase(EventType.MediaEvent)]
    [TestCase(EventType.SportingEvent)]
    [TestCase(EventType.VirtualEvent)]
    public async Task Handle_QueryContainsFilterByType_ShouldReturnOnlyEventsWithSpecifiedType(EventType type)
    {
        Context.Events = TestHelpers.MockDbSet(
            CreateBasicEvent(1, type: EventType.Ceremony), CreateBasicEvent(2, type: EventType.Convention),
            CreateBasicEvent(3, type: EventType.Festival), CreateBasicEvent(3, type: EventType.Happening),
            CreateBasicEvent(3, type: EventType.Party), CreateBasicEvent(3, type: EventType.MediaEvent),
            CreateBasicEvent(3, type: EventType.SportingEvent), CreateBasicEvent(3, type: EventType.VirtualEvent));

        var query = new GetEventsQuery
        {
            Type = type
        };

        var handler = CreateHandler();

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.IsTrue(result.Events.All(e => e.Type == type));
    }

    [Test]
    public async Task Handle_QueryContainsInvalidEventType_ShouldReturnNoneOfEvents()
    {
        Context.Events = TestHelpers.MockDbSet(CreateBasicEvent(1), CreateBasicEvent(2), CreateBasicEvent(3));

        var query = new GetEventsQuery
        {
            Type = (EventType) 100
        };

        var handler = CreateHandler();

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.AreEqual(0, result.Count);
        Assert.AreEqual(0, result.Events.Count);
    }

    [TestCase(true)]
    [TestCase(false)]
    public async Task Handle_QueryContainsFilterByPrivate(bool isPrivate)
    {
        Context.Events = TestHelpers.MockDbSet(CreateBasicEvent(1, isPrivate: true), CreateBasicEvent(2, isPrivate: false));

        var query = new GetEventsQuery
        {
            IsPrivate = isPrivate
        };

        var handler = CreateHandler();

        var result = await handler.Handle(query, CancellationToken.None);
        
        Assert.IsTrue(result.Events.All(e => e.IsPrivate == isPrivate));
    }

    [TestCase(GetEventsSortBy.Name)]
    [TestCase(GetEventsSortBy.Duration)]
    [TestCase(GetEventsSortBy.BeginsAt)]
    [TestCase(GetEventsSortBy.CreatedAt)]
    public async Task Handle_QueryContainsSortByParameterAndOtherNeededOnes_ShouldReturnEventsSortedBySpecifiedProperty(GetEventsSortBy sortBy)
    {
        Context.Events = TestHelpers.MockDbSet(
            CreateBasicEvent(1, name: "aaa", duration: TimeSpan.FromHours(1), beginsAt: DateTime.UtcNow, createdAt: DateTime.UtcNow),
            CreateBasicEvent(2, name: "bbb", duration: TimeSpan.FromHours(2), beginsAt: DateTime.UtcNow.AddDays(1), createdAt: DateTime.UtcNow.AddDays(1)),
            CreateBasicEvent(3, name: "ccc", duration: TimeSpan.FromHours(3), beginsAt: DateTime.UtcNow.AddDays(2), createdAt: DateTime.UtcNow.AddDays(2))
            );

        var query = new GetEventsQuery
        {
            SortBy = sortBy,
            SortingOrder = SortingOrder.Ascending
        };

        var handler = CreateHandler();

        var result = await handler.Handle(query, CancellationToken.None);

        Func<CollectionEventDto, object> keySelector = sortBy switch
        {
            GetEventsSortBy.Name => e => e.Name,
            GetEventsSortBy.Duration => e => e.Duration,
            GetEventsSortBy.BeginsAt => e => e.BeginsAt,
            GetEventsSortBy.CreatedAt => e => e.CreatedAt,
            _ => throw new ArgumentOutOfRangeException(nameof(sortBy))
        };
        
        var firstEvent = result.Events.First();
        var lastEvent = result.Events.Last();

        Assert.IsTrue(result.Events.MinBy(keySelector)?.Id == firstEvent.Id &&
                      result.Events.MaxBy(keySelector)?.Id == lastEvent.Id);
    }

    [Test]
    public void Handle_QueryWithInvalidSortBy_ShouldThrowBadRequestException()
    {
        Context.Events = TestHelpers.MockDbSet(Array.Empty<Event>());

        var query = new GetEventsQuery
        {
            SortBy = (GetEventsSortBy) 100,
            SortingOrder = SortingOrder.Ascending
        };

        var handler = CreateHandler();

        Assert.ThrowsAsync<BadRequestException>(() => handler.Handle(query, CancellationToken.None));
    }

    [TestCase(SortingOrder.Ascending)]
    [TestCase(SortingOrder.Descending)]
    public async Task Handle_QueryContainsSortingOrderAndOtherNeededParameters_ShouldReturnEventsSortedInSpecifiedOrder(SortingOrder sortingOrder)
    {
        Context.Events = TestHelpers.MockDbSet(
            CreateBasicEvent(1, name: "aaa", duration: TimeSpan.FromHours(1), beginsAt: DateTime.UtcNow,
                createdAt: DateTime.UtcNow),
            CreateBasicEvent(2, name: "bbb", duration: TimeSpan.FromHours(2), beginsAt: DateTime.UtcNow.AddDays(1),
                createdAt: DateTime.UtcNow.AddDays(1)),
            CreateBasicEvent(3, name: "ccc", duration: TimeSpan.FromHours(3), beginsAt: DateTime.UtcNow.AddDays(2),
                createdAt: DateTime.UtcNow.AddDays(2))
        );

        var query = new GetEventsQuery
        {
            SortingOrder = sortingOrder,
            SortBy = GetEventsSortBy.BeginsAt
        };

        var handler = CreateHandler();

        var result = await handler.Handle(query, CancellationToken.None);
        
        var firstEvent = result.Events.First();
        var lastEvent = result.Events.Last();

        var earliestEvent = result.Events.MinBy(e => e.BeginsAt);
        var latestEvent = result.Events.MaxBy(e => e.BeginsAt);
        
        var comparisonResult = sortingOrder switch
        {
            SortingOrder.Ascending => earliestEvent?.Id == firstEvent.Id && latestEvent?.Id == lastEvent.Id,
            SortingOrder.Descending => earliestEvent?.Id == lastEvent.Id && latestEvent?.Id == firstEvent.Id,
            _ => throw new ArgumentOutOfRangeException(nameof(sortingOrder))
        };

        Assert.IsTrue(comparisonResult);
    }

    [Test]
    public void Handle_QueryWithInvalidSortingOrder_ShouldThrowBadRequestException()
    {
        Context.Events = TestHelpers.MockDbSet(Array.Empty<Event>());
        
        var query = new GetEventsQuery
        {
            SortingOrder = (SortingOrder) 100,
            SortBy = GetEventsSortBy.Name
        };

        var handler = CreateHandler();

        Assert.ThrowsAsync<BadRequestException>(() => handler.Handle(query, CancellationToken.None));
    }

    [Test]
    public async Task Handle_QueryWithOneOmittedParameterForSorting_ShouldNotSortEvents()
    {
        Context.Events = TestHelpers.MockDbSet(CreateBasicEvent(1, beginsAt: DateTime.UtcNow.AddDays(3)),
            CreateBasicEvent(2, beginsAt: DateTime.UtcNow.AddDays(-10)),
            CreateBasicEvent(3, beginsAt: DateTime.UtcNow));

        var query = new GetEventsQuery
        {
            SortBy = GetEventsSortBy.BeginsAt
        };

        var handler = CreateHandler();

        var result = await handler.Handle(query, CancellationToken.None);
        
        Assert.AreEqual(Context.Events.First().Id, result.Events.First().Id);
        Assert.AreEqual(Context.Events.ElementAt(1).Id, result.Events.ElementAt(1).Id);
        Assert.AreEqual(Context.Events.Last().Id, result.Events.Last().Id);
    }

    [Test]
    public async Task Handle_QueryContainsPagination_ShouldReturnPaginatedEvents()
    {
        const long firstEventId = 1;
        const long secondEventId = 2;
        const long thirdEventId = 3;
        const long fourthEventId = 4;
        const long fifthEventId = 5;
        const long sixthEventId = 6;

        const int page = 2;
        const int pageSize = 3;

        Context.Events = TestHelpers.MockDbSet(CreateBasicEvent(firstEventId), CreateBasicEvent(secondEventId),
            CreateBasicEvent(thirdEventId), CreateBasicEvent(fourthEventId),
            CreateBasicEvent(fifthEventId), CreateBasicEvent(sixthEventId));

        var query = new GetEventsQuery
        {
            Page = page,
            PageSize = pageSize
        };

        var handler = CreateHandler();

        var result = await handler.Handle(query, CancellationToken.None);
        
        Assert.AreEqual(Context.Events.Count(), result.Count);
        Assert.AreEqual(pageSize, result.Events.Count);
        Assert.IsTrue(result.Events.All(e => e.Id is fourthEventId or fifthEventId or sixthEventId));
    }

    [Test]
    public async Task Handle_QueryWithOneOmittedParameterForPagination_ShouldReturnAllEvents()
    {
        Context.Events = TestHelpers.MockDbSet(CreateBasicEvent(1), CreateBasicEvent(2), CreateBasicEvent(3),
            CreateBasicEvent(4));

        var query = new GetEventsQuery
        {
            Page = 100
        };

        var handler = CreateHandler();

        var result = await handler.Handle(query, CancellationToken.None);
        
        Assert.AreEqual(Context.Events.Count(), result.Count);
        Assert.AreEqual(Context.Events.Count(), result.Events.Count);
    }

    private GetEventsQueryHandler CreateHandler() => new(Context, Mapper);

    private Event CreateBasicEvent(long id, EventType type = EventType.Ceremony, bool isPrivate = false,
        string name = "Name", TimeSpan duration = new(), DateTime beginsAt = new(), DateTime createdAt = new()) => new()
    {
        Id = id,
        Name = name,
        Type = type,
        Address = "Long street 60",
        Comment = "Comment",
        Duration = duration,
        IsPrivate = isPrivate,
        BeginsAt = beginsAt,
        CreatedAt = createdAt
    };
}