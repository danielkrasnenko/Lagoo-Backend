using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lagoo.BusinessLogic.CommandsAndQueries.Events.Queries.GetEvents;
using Lagoo.BusinessLogic.Common.Enums;
using Lagoo.BusinessLogic.Common.Exceptions.Api;
using Lagoo.BusinessLogic.UnitTests.Common.Base;
using Lagoo.Domain.Enums;
using NSubstitute;
using NUnit.Framework;

namespace Lagoo.BusinessLogic.UnitTests.CommandsAndQueries.Events.Queries.GetEvents;

/// <summary>
///   Tests for <see cref="GetEventsQueryHandler"/>
/// </summary>
[TestFixture]
public class GetEventsQueryHandlerTests : TestsBase
{
    [Test]
    public async Task Handle_ThereAreNoEventsInDatabase_ShouldReturnEmptyList()
    {
        var query = new GetEventsQuery();

        EventRepository.GetAllAsync(query, CancellationToken.None).ReturnsForAnyArgs(new GetEventsResponseDto());
        
        var handler = CreateHandler();

        var result = await handler.Handle(query, CancellationToken.None);
        
        Assert.IsEmpty(result.Events);
        Assert.AreEqual(0, result.Count);
    }

    [Test]
    public async Task Handle_ThereAreEventsInDatabase_ShouldReturnListWithEvents()
    {
        var query = new GetEventsQuery();

        EventRepository.GetAllAsync(query, CancellationToken.None).ReturnsForAnyArgs(new GetEventsResponseDto
        {
            Events = new List<CollectionEventDto> { CreateBasicEvent(1), CreateBasicEvent(2) },
            Count = 2
        });

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
        var query = new GetEventsQuery
        {
            Type = type
        };

        EventRepository.GetAllAsync(query, CancellationToken.None).ReturnsForAnyArgs(new GetEventsResponseDto
        {
            Events = new List<CollectionEventDto>
            {
                CreateBasicEvent(1, type: EventType.Ceremony), CreateBasicEvent(2, type: EventType.Convention),
                CreateBasicEvent(3, type: EventType.Festival), CreateBasicEvent(3, type: EventType.Happening),
                CreateBasicEvent(3, type: EventType.Party), CreateBasicEvent(3, type: EventType.MediaEvent),
                CreateBasicEvent(3, type: EventType.SportingEvent), CreateBasicEvent(3, type: EventType.VirtualEvent)
            },
            Count = 8
        });

        var handler = CreateHandler();

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.IsTrue(result.Events.All(e => e.Type == type));
    }

    [Test]
    public async Task Handle_QueryContainsInvalidEventType_ShouldReturnNoneOfEvents()
    {
        var query = new GetEventsQuery
        {
            Type = (EventType) 100
        };

        EventRepository.GetAllAsync(query, CancellationToken.None).ReturnsForAnyArgs(new GetEventsResponseDto
        {
            Events = new List<CollectionEventDto>(),
            Count = 0
        });

        var handler = CreateHandler();

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.AreEqual(0, result.Count);
        Assert.AreEqual(0, result.Events.Count);
    }

    [TestCase(true)]
    [TestCase(false)]
    public async Task Handle_QueryContainsFilterByPrivate_ShouldReturnOnlyEventsWithSpecifiedPrivateProperty(bool isPrivate)
    {
        var query = new GetEventsQuery
        {
            IsPrivate = isPrivate
        };

        EventRepository.GetAllAsync(query, CancellationToken.None).ReturnsForAnyArgs(new GetEventsResponseDto
        {
            Events = new List<CollectionEventDto>
                { CreateBasicEvent(1, isPrivate: true)},
            Count = 1
        });

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
        var query = new GetEventsQuery
        {
            SortBy = sortBy,
            SortingOrder = SortingOrder.Ascending
        };

        EventRepository.GetAllAsync(query, CancellationToken.None).ReturnsForAnyArgs(new GetEventsResponseDto
        {
            Events = new List<CollectionEventDto>
            {
                CreateBasicEvent(1, name: "aaa", duration: TimeSpan.FromHours(1), beginsAt: DateTime.UtcNow,
                    createdAt: DateTime.UtcNow),
                CreateBasicEvent(2, name: "bbb", duration: TimeSpan.FromHours(2), beginsAt: DateTime.UtcNow.AddDays(1),
                    createdAt: DateTime.UtcNow.AddDays(1)),
                CreateBasicEvent(3, name: "ccc", duration: TimeSpan.FromHours(3), beginsAt: DateTime.UtcNow.AddDays(2),
                    createdAt: DateTime.UtcNow.AddDays(2))
            },
            Count = 3
        });

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
    public void Handle_QueryWithInvalidSortByParameter_ShouldThrowBadRequestException()
    {
        var query = new GetEventsQuery
        {
            SortBy = (GetEventsSortBy) 100,
            SortingOrder = SortingOrder.Ascending
        };

        EventRepository.GetAllAsync(query, CancellationToken.None).ReturnsForAnyArgs(new GetEventsResponseDto
        {
            Events = new List<CollectionEventDto>(),
            Count = 0
        });
        
        var handler = CreateHandler();

        Assert.ThrowsAsync<BadRequestException>(() => handler.Handle(query, CancellationToken.None));
    }

    [TestCase(SortingOrder.Ascending)]
    [TestCase(SortingOrder.Descending)]
    public async Task Handle_QueryContainsSortingOrderAndOtherNeededParameters_ShouldReturnEventsSortedInSpecifiedOrder(SortingOrder sortingOrder)
    {
        var query = new GetEventsQuery
        {
            SortingOrder = sortingOrder,
            SortBy = GetEventsSortBy.BeginsAt
        };
        
        EventRepository.GetAllAsync(query, CancellationToken.None).ReturnsForAnyArgs(new GetEventsResponseDto
        {
            Events = new List<CollectionEventDto>
            {
                CreateBasicEvent(1, name: "aaa", duration: TimeSpan.FromHours(1), beginsAt: DateTime.UtcNow,
                    createdAt: DateTime.UtcNow),
                CreateBasicEvent(2, name: "bbb", duration: TimeSpan.FromHours(2), beginsAt: DateTime.UtcNow.AddDays(1),
                    createdAt: DateTime.UtcNow.AddDays(1)),
                CreateBasicEvent(3, name: "ccc", duration: TimeSpan.FromHours(3), beginsAt: DateTime.UtcNow.AddDays(2),
                    createdAt: DateTime.UtcNow.AddDays(2))
            },
            Count = 3
        });

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
        var query = new GetEventsQuery
        {
            SortingOrder = (SortingOrder) 100,
            SortBy = GetEventsSortBy.Name
        };
        
        EventRepository.GetAllAsync(query, CancellationToken.None).ReturnsForAnyArgs(new GetEventsResponseDto
        {
            Events = new List<CollectionEventDto>(),
            Count = 0
        });

        var handler = CreateHandler();

        Assert.ThrowsAsync<BadRequestException>(() => handler.Handle(query, CancellationToken.None));
    }

    [Test]
    public async Task Handle_QueryWithOneOmittedParameterForSorting_ShouldNotSortEvents()
    {
        var events = new List<CollectionEventDto>
        {
            CreateBasicEvent(1, beginsAt: DateTime.UtcNow.AddDays(3)),
            CreateBasicEvent(2, beginsAt: DateTime.UtcNow.AddDays(-10)),
            CreateBasicEvent(3, beginsAt: DateTime.UtcNow)
        };

        var query = new GetEventsQuery
        {
            SortBy = GetEventsSortBy.BeginsAt
        };
        
        EventRepository.GetAllAsync(query, CancellationToken.None).ReturnsForAnyArgs(new GetEventsResponseDto
        {
            Events = events,
            Count = 3
        });

        var handler = CreateHandler();

        var result = await handler.Handle(query, CancellationToken.None);
        
        Assert.AreEqual(events.First().Id, result.Events.First().Id);
        Assert.AreEqual(events.ElementAt(1).Id, result.Events.ElementAt(1).Id);
        Assert.AreEqual(events.Last().Id, result.Events.Last().Id);
    }

    [Test]
    public async Task Handle_QueryContainsPaginationForRandomPage_ShouldReturnPaginatedEventsBySpecifiedPageAndItsSize()
    {
        const long firstEventId = 1;
        const long secondEventId = 2;
        const long thirdEventId = 3;
        const long fourthEventId = 4;
        const long fifthEventId = 5;
        const long sixthEventId = 6;

        const int page = 2;
        const int pageSize = 3;

        var events = new List<CollectionEventDto>
        {
            CreateBasicEvent(firstEventId), CreateBasicEvent(secondEventId),
            CreateBasicEvent(thirdEventId), CreateBasicEvent(fourthEventId),
            CreateBasicEvent(fifthEventId), CreateBasicEvent(sixthEventId)
        };

        var query = new GetEventsQuery
        {
            Page = page,
            PageSize = pageSize
        };
        
        EventRepository.GetAllAsync(query, CancellationToken.None).ReturnsForAnyArgs(new GetEventsResponseDto
        {
            Events = events,
            Count = 6
        });

        var handler = CreateHandler();

        var result = await handler.Handle(query, CancellationToken.None);
        
        Assert.AreEqual(events.Count, result.Count);
        Assert.AreEqual(pageSize, result.Events.Count);
        Assert.IsTrue(result.Events.All(e => e.Id is fourthEventId or fifthEventId or sixthEventId));
    }
    
    [Test]
    public async Task Handle_QueryContainsPaginationForNextOrPreviousPage_ShouldReturnPaginatedEventsAccordingToSpecifiedParameters()
    {
        const long firstEventId = 1;
        const long secondEventId = 2;
        const long thirdEventId = 3;
        const long fourthEventId = 4;
        const long fifthEventId = 5;
        const long sixthEventId = 6;

        const int pageSize = 3;

        var events = new List<CollectionEventDto>
        {
            CreateBasicEvent(firstEventId), CreateBasicEvent(secondEventId),
            CreateBasicEvent(thirdEventId), CreateBasicEvent(fourthEventId),
            CreateBasicEvent(fifthEventId), CreateBasicEvent(sixthEventId)
        };

        var query = new GetEventsQuery
        {
            LastFetchedEventId = thirdEventId,
            PageSize = pageSize
        };

        EventRepository.GetAllAsync(query, CancellationToken.None).ReturnsForAnyArgs(new GetEventsResponseDto
        {
            Events = events,
            Count = 6
        });
        
        var handler = CreateHandler();

        var result = await handler.Handle(query, CancellationToken.None);
        
        Assert.AreEqual(events.Count, result.Count);
        Assert.AreEqual(pageSize, result.Events.Count);
        Assert.IsTrue(result.Events.All(e => e.Id > thirdEventId));
    }

    [Test]
    public async Task Handle_QueryContainsPaginationAndSorting_ShouldReturnAGroupOfSortedEvents()
    {
        const long firstEventId = 1;
        const long secondEventId = 2;
        const long thirdEventId = 3;
        const long fourthEventId = 4;
        const long fifthEventId = 5;
        const long sixthEventId = 6;

        const int pageSize = 3;

        var events = new List<CollectionEventDto>
        {
            CreateBasicEvent(firstEventId, beginsAt: DateTime.UtcNow), CreateBasicEvent(secondEventId, beginsAt: DateTime.UtcNow.AddDays(1)),
            CreateBasicEvent(thirdEventId, beginsAt: DateTime.UtcNow.AddDays(2)), CreateBasicEvent(fourthEventId, beginsAt: DateTime.UtcNow.AddDays(3)),
            CreateBasicEvent(fifthEventId, beginsAt: DateTime.UtcNow.AddDays(4)), CreateBasicEvent(sixthEventId, beginsAt: DateTime.UtcNow.AddDays(5))
        };

        var query = new GetEventsQuery
        {
            LastFetchedEventId = 0,
            PageSize = pageSize,
            SortingOrder = SortingOrder.Descending,
            SortBy = GetEventsSortBy.BeginsAt
        };
        
        EventRepository.GetAllAsync(query, CancellationToken.None).ReturnsForAnyArgs(new GetEventsResponseDto
        {
            Events = events,
            Count = 6
        });

        var handler = CreateHandler();

        var result = await handler.Handle(query, CancellationToken.None);
        
        Assert.AreEqual(events.Count, result.Count);
        Assert.AreEqual(pageSize, result.Events.Count);
        Assert.IsTrue(result.Events.First().Id == sixthEventId);
        Assert.IsTrue(result.Events.ElementAt(1).Id == fifthEventId);
        Assert.IsTrue(result.Events.ElementAt(2).Id == fourthEventId);
    }

    [Test]
    public async Task Handle_QueryWithSpecifiedPageParameterAndOmittedPageSizeParameterForPagination_ShouldReturnAllEvents()
    {
        var events = new List<CollectionEventDto>
        {
            CreateBasicEvent(1), CreateBasicEvent(2),
            CreateBasicEvent(3), CreateBasicEvent(4)
        };

        var query = new GetEventsQuery
        {
            Page = 100
        };
        
        EventRepository.GetAllAsync(query, CancellationToken.None).ReturnsForAnyArgs(new GetEventsResponseDto
        {
            Events = events,
            Count = 4
        });

        var handler = CreateHandler();

        var result = await handler.Handle(query, CancellationToken.None);
        
        Assert.AreEqual(events.Count, result.Count);
        Assert.AreEqual(events.Count, result.Events.Count);
    }
    
    [Test]
    public async Task Handle_QueryWithSpecifiedLastFetchedEventIdParameterAndOmittedPageSizeParameterForPagination_ShouldReturnAllEvents()
    {
        var events = new List<CollectionEventDto>
        {
            CreateBasicEvent(1), CreateBasicEvent(2),
            CreateBasicEvent(3), CreateBasicEvent(4)
        };

        var query = new GetEventsQuery
        {
            LastFetchedEventId = 55
        };
        
        EventRepository.GetAllAsync(query, CancellationToken.None).ReturnsForAnyArgs(new GetEventsResponseDto
        {
            Events = events,
            Count = 4
        });

        var handler = CreateHandler();

        var result = await handler.Handle(query, CancellationToken.None);
        
        Assert.AreEqual(events.Count, result.Count);
        Assert.AreEqual(events.Count, result.Events.Count);
    }
    
    [Test]
    public async Task Handle_QueryWithOmittedPageAndLastFetchedEventIdParametersForPagination_ShouldReturnAllEvents()
    {
        var events = new List<CollectionEventDto>
        {
            CreateBasicEvent(1), CreateBasicEvent(2),
            CreateBasicEvent(3), CreateBasicEvent(4)
        };

        var query = new GetEventsQuery
        {
            PageSize = 3
        };
        
        EventRepository.GetAllAsync(query, CancellationToken.None).ReturnsForAnyArgs(new GetEventsResponseDto
        {
            Events = events,
            Count = 4
        });

        var handler = CreateHandler();

        var result = await handler.Handle(query, CancellationToken.None);
        
        Assert.AreEqual(events.Count, result.Count);
        Assert.AreEqual(events.Count, result.Events.Count);
    }

    private GetEventsQueryHandler CreateHandler() => new(EventRepository);

    private CollectionEventDto CreateBasicEvent(long id, EventType type = EventType.Ceremony, bool isPrivate = false,
        string name = "Name", TimeSpan duration = new(), DateTime beginsAt = new(), DateTime createdAt = new()) => new()
    {
        Id = id,
        Name = name,
        Type = type,
        Address = "Long street 60",
        Duration = duration,
        IsPrivate = isPrivate,
        BeginsAt = beginsAt,
        CreatedAt = createdAt
    };
}