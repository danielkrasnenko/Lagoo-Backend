using System.Threading;
using Lagoo.BusinessLogic.CommandsAndQueries.Events.Commands.DeleteEvent;
using Lagoo.BusinessLogic.Common.Exceptions.Api;
using Lagoo.BusinessLogic.UnitTests.Common.Base;
using Lagoo.BusinessLogic.UnitTests.Common.Helpers;
using Lagoo.Domain.Entities;
using NSubstitute;
using NUnit.Framework;

namespace Lagoo.BusinessLogic.UnitTests.CommandsAndQueries.Events.Commands.DeleteEvent;

/// <summary>
///   Tests for <see cref="DeleteEventCommandHandler"/>
/// </summary>
[TestFixture]
public class DeleteEventCommandHandlerTests : TestsBase
{
    private const long DefaultEventId = 1;
    
    [Test]
    public void Handle_EventExists_ShouldDeleteEvent()
    {
        EventRepository.DeleteAsync(DefaultEventId, CancellationToken.None).ReturnsForAnyArgs(true);
        
        var command = new DeleteEventCommand { EventId = DefaultEventId };

        var handler = CreateHandler();
        
        Assert.DoesNotThrowAsync(() => handler.Handle(command, CancellationToken.None));
    }

    [Test]
    public void Handle_EventDoesNotExist_ShouldThrowNotFoundException()
    {
        EventRepository.DeleteAsync(DefaultEventId, CancellationToken.None).ReturnsForAnyArgs(false);
        
        var command = new DeleteEventCommand { EventId = 100 };

        var handler = CreateHandler();

        Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, CancellationToken.None));
    }

    private DeleteEventCommandHandler CreateHandler() => new(EventRepository);
}