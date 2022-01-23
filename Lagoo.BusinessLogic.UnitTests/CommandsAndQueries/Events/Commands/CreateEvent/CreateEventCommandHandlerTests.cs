using System;
using System.Threading;
using System.Threading.Tasks;
using Lagoo.BusinessLogic.CommandsAndQueries.Events.Commands.CreateEvent;
using Lagoo.BusinessLogic.UnitTests.Common.Base;
using Lagoo.BusinessLogic.UnitTests.Common.Helpers;
using Lagoo.Domain.Entities;
using Lagoo.Domain.Enums;
using NUnit.Framework;

namespace Lagoo.BusinessLogic.UnitTests.CommandsAndQueries.Events.Commands.CreateEvent;

/// <summary>
///   Tests for <see cref="CreateEventCommandHandler"/>
/// </summary>
[TestFixture]
public class CreateEventCommandHandlerTests : TestsBase
{
    [SetUp]
    public void SetUp()
    {
        Context.Events = TestHelpers.MockDbSet(Array.Empty<Event>());
    }
    
    [Test]
    public async Task Handle_ValidDataForNewEventIsProvided_ShouldCreateNewEvent()
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

    private CreateEventCommandHandler CreateHandler() => new(Context, Mapper);
}