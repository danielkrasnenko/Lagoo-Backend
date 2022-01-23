using System.Threading;
using Lagoo.BusinessLogic.CommandsAndQueries.Events.Commands.DeleteEvent;
using Lagoo.BusinessLogic.Common.Exceptions.Api;
using Lagoo.BusinessLogic.UnitTests.Common.Base;
using Lagoo.BusinessLogic.UnitTests.Common.Helpers;
using Lagoo.Domain.Entities;
using NUnit.Framework;

namespace Lagoo.BusinessLogic.UnitTests.CommandsAndQueries.Events.Commands.DeleteEvent;

/// <summary>
///   Tests for <see cref="DeleteEventCommandHandler"/>
/// </summary>
[TestFixture]
public class DeleteEventCommandHandlerTests : TestsBase
{
    private const long DefaultEventId = 1;
    
    [SetUp]
    public void SetUp()
    {
        Context.Events = TestHelpers.MockDbSet(new Event { Id = DefaultEventId });
    }
    
    [Test]
    public void Handle_EventExists_ShouldDeleteEvent()
    {
        var command = new DeleteEventCommand { Id = DefaultEventId };

        var handler = CreateHandler();
        
        Assert.DoesNotThrowAsync(() => handler.Handle(command, CancellationToken.None));
    }

    [Test]
    public void Handle_EventDoesNotExist_ShouldThrowNotFoundException()
    {
        var command = new DeleteEventCommand { Id = 100 };

        var handler = CreateHandler();

        Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, CancellationToken.None));
    }

    private DeleteEventCommandHandler CreateHandler() => new(Context);
}