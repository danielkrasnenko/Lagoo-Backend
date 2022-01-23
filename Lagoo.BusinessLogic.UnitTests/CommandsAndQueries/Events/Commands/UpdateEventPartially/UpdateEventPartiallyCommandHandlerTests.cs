using System;
using System.Threading;
using System.Threading.Tasks;
using Lagoo.BusinessLogic.CommandsAndQueries.Events.Commands.UpdateEventPartially;
using Lagoo.BusinessLogic.Common.Exceptions.Api;
using Lagoo.BusinessLogic.UnitTests.Common.Base;
using Lagoo.BusinessLogic.UnitTests.Common.Helpers;
using Lagoo.Domain.Entities;
using Lagoo.Domain.Enums;
using NUnit.Framework;

namespace Lagoo.BusinessLogic.UnitTests.CommandsAndQueries.Events.Commands.UpdateEventPartially;

/// <summary>
///   Tests for <see cref="UpdateEventPartiallyCommandHandler"/>
/// </summary>
[TestFixture]
public class UpdateEventPartiallyCommandHandlerTests : TestsBase
{
    [Test]
    public async Task Handle_CommandContainsValidDataAndEventExists_ShouldReturnPartiallyUpdatedEvent()
    {
        const long eventId = 1;

        Context.Events = TestHelpers.MockDbSet(new Event { Id = eventId, Type = EventType.Festival });

        var command = new UpdateEventPartiallyCommand
        {
            Id = eventId,
            Name = "New name",
            Type = EventType.MediaEvent,
            Address = "New address",
            Comment = "New Comment",
            Duration = TimeSpan.FromHours(3),
            IsPrivate = true,
            BeginsAt = DateTime.UtcNow
        };

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
        Context.Events = TestHelpers.MockDbSet(Array.Empty<Event>());

        var command = new UpdateEventPartiallyCommand { Id = 1 };

        var handler = CreateHandler();

        Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, CancellationToken.None));
    }

    private UpdateEventPartiallyCommandHandler CreateHandler() => new(Context, Mapper);
}