using System;
using System.Threading;
using System.Threading.Tasks;
using Lagoo.BusinessLogic.CommandsAndQueries.Events.Commands.CreateEvent;
using Lagoo.BusinessLogic.CommandsAndQueries.Events.Common.Dtos;
using Lagoo.BusinessLogic.UnitTests.Common.Base;
using Lagoo.Domain.Enums;
using NSubstitute;
using NUnit.Framework;

namespace Lagoo.BusinessLogic.UnitTests.CommandsAndQueries.Events.Commands.CreateEvent;

/// <summary>
///   Tests for <see cref="CreateEventCommandHandler"/>
/// </summary>
[TestFixture]
public class CreateEventCommandHandlerTests : TestsBase
{
    [Test]
    public async Task Handle_CommandContainsValidDataForNewEvent_ShouldCreateNewEvent()
    {
        var command = new CreateEventCommand
        {
            Name = "Name",
            Type = EventType.Ceremony,
            Address = "Long street 19",
            Comment = "Nice",
            Duration = TimeSpan.FromHours(2),
            BeginsAt = DateTime.UtcNow,
            IsPrivate = true
        };

        EventRepository.CreateAsync(command, CancellationToken.None).ReturnsForAnyArgs(new ReadEventDto
        {
            Name = command.Name,
            Type = command.Type,
            Address = command.Address,
            Comment = command.Comment,
            Duration = command.Duration,
            BeginsAt = command.BeginsAt,
            IsPrivate = command.IsPrivate
        });
        
        var handler = CreateHandler();

        var result = await handler.Handle(command, CancellationToken.None);
        
        Assert.AreEqual(command.Name, result.Name);
        Assert.AreEqual(command.Type, result.Type);
        Assert.AreEqual(command.Address, result.Address);
        Assert.AreEqual(command.Comment, result.Comment);
        Assert.AreEqual(command.Duration, result.Duration);
        Assert.AreEqual(command.BeginsAt, result.BeginsAt);
        Assert.AreEqual(command.IsPrivate, result.IsPrivate);
        Assert.That(result.Id, IsNotNullOrEmpty);
        Assert.That(result.CreatedAt, IsNotNullOrEmpty);
        Assert.IsNull(result.LastModifiedAt);
    }

    private CreateEventCommandHandler CreateHandler() => new(EventRepository);
}