using System;
using System.Threading;
using System.Threading.Tasks;
using Lagoo.BusinessLogic.CommandsAndQueries.Events.Commands.UpdateEvent;
using Lagoo.BusinessLogic.CommandsAndQueries.Events.Common.Dtos;
using Lagoo.BusinessLogic.Common.Exceptions.Api;
using Lagoo.BusinessLogic.UnitTests.Common.Base;
using Lagoo.Domain.Enums;
using NSubstitute;
using NUnit.Framework;

namespace Lagoo.BusinessLogic.UnitTests.CommandsAndQueries.Events.Commands.UpdateEvent;

/// <summary>
///   Tests for <see cref="UpdateEventCommandHandler"/>
/// </summary>
[TestFixture]
public class UpdateEventCommandHandlerTests : TestsBase
{
    [Test]
    public async Task Handle_CommandContainsValidDataAndEventExists_ShouldReturnUpdatedEvent()
    {
        const long eventId = 1;
        
        var command = new UpdateEventCommand
        {
            Id = eventId,
            Name = "New name",
            Type = EventType.Festival,
            Address = "New address",
            Comment = "New Comment",
            Duration = TimeSpan.FromHours(3),
            IsPrivate = true,
            BeginsAt = DateTime.UtcNow
        };

        EventRepository.UpdateAsync(new UpdateEventCommand(), CancellationToken.None).ReturnsForAnyArgs(new ReadEventDto
        {
            Id = eventId,
            Name = command.Name,
            Type = command.Type,
            Address = command.Address,
            Comment = command.Comment,
            Duration = command.Duration,
            IsPrivate = command.IsPrivate,
            BeginsAt = command.BeginsAt
        });

        var handler = CreateHandler();

        var result = await handler.Handle(command, CancellationToken.None);
        
        Assert.AreEqual(eventId, result.Id);
        Assert.AreEqual(command.Name, result.Name);
        Assert.AreEqual(command.Type, result.Type);
        Assert.AreEqual(command.Address, result.Address);
        Assert.AreEqual(command.Comment, result.Comment);
        Assert.AreEqual(command.Duration, result.Duration);
        Assert.AreEqual(command.IsPrivate, result.IsPrivate);
        Assert.AreEqual(command.BeginsAt, result.BeginsAt);
        Assert.IsNotNull(result.LastModifiedAt);
    }

    [Test]
    public void Handle_EventDoesNotExist_ShouldThrowNotFoundException()
    {
        EventRepository.UpdateAsync(new UpdateEventCommand(), CancellationToken.None).ReturnsForAnyArgs(null as object);

        var command = new UpdateEventCommand { Id = 1 };

        var handler = CreateHandler();

        Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, CancellationToken.None));
    }
    
    private UpdateEventCommandHandler CreateHandler() => new(EventRepository);
}