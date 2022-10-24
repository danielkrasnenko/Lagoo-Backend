using System;
using System.Threading;
using System.Threading.Tasks;
using Lagoo.BusinessLogic.CommandsAndQueries.Accounts.Commands.CreateAuthTokens;
using Lagoo.BusinessLogic.CommandsAndQueries.Accounts.Commands.LoginUser;
using Lagoo.BusinessLogic.Common.Exceptions.Api;
using Lagoo.BusinessLogic.UnitTests.CommandsAndQueries.Accounts.Common;
using NSubstitute;
using NUnit.Framework;

namespace Lagoo.BusinessLogic.UnitTests.CommandsAndQueries.Accounts.Commands.LoginUser;

/// <summary>
///   Tests for <see cref="LoginUserCommandHandler"/>
/// </summary>
[TestFixture]
public class LoginUserCommandHandlerTests : AccountTestsBase
{
    [SetUp]
    public void SetUp()
    {
        SetServicesDefaultBehaviour();
    }

    [Test]
    public async Task Handle_CommandContainsDeviceIdAndValidData_ShouldReturnAuthTokenDataForExistingDevice()
    {
        var command = GenerateCommandWithValidDefaultData(deviceId: DefaultDeviceId);

        var handler = CreateHandler();

        var result = await handler.Handle(command, CancellationToken.None);
        
        AssertAuthenticationDataDtoContainsDefaultData(result);
        Assert.AreEqual(DefaultDeviceId, result.DeviceId);
    }

    [Test]
    public async Task Handle_CommandContainsOnlyValidData_ShouldReturnAuthTokenDataForNewDevice()
    {
        Mediator.Send(new CreateAuthTokensCommand(DefaultUser)).ReturnsForAnyArgs(GenerateDefaultAuthDataDto(NewDeviceId));
        
        var command = GenerateCommandWithValidDefaultData();

        var handler = CreateHandler();

        var result = await handler.Handle(command, CancellationToken.None);
        
        AssertAuthenticationDataDtoContainsDefaultData(result);
        Assert.AreEqual(NewDeviceId, result.DeviceId);
    }

    [Test]
    public void Handle_UserManagerCannotUserFindByEmail_ShouldThrowBadRequestException()
    {
        UserRepository.FindByEmailAsync("").ReturnsForAnyArgs(null as object);

        var command = GenerateCommandWithValidDefaultData();

        var handler = CreateHandler();

        Assert.ThrowsAsync<BadRequestException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Test]
    public void Handle_CommandWithWrongPassword_ShouldThrowBadRequestException()
    {
        UserRepository.CheckPasswordAsync(DefaultUser, "").ReturnsForAnyArgs(false);
        
        var command = GenerateCommandWithValidDefaultData();
        
        var handler = CreateHandler();

        Assert.ThrowsAsync<BadRequestException>(() => handler.Handle(command, CancellationToken.None));
    }
    
    private LoginUserCommandHandler CreateHandler() => new(UserRepository, Mediator);
    
    private void SetServicesDefaultBehaviour()
    {
        UserRepository.FindByEmailAsync(default).ReturnsForAnyArgs(DefaultUser);
        UserRepository.CheckPasswordAsync(DefaultUser, default).ReturnsForAnyArgs(true);
        Mediator.Send(new CreateAuthTokensCommand(DefaultUser)).ReturnsForAnyArgs(GenerateDefaultAuthDataDto(DefaultDeviceId));
    }
    
    private LoginUserCommand GenerateCommandWithValidDefaultData(string email = DefaultUserEmail, string password = ValidPassword, Guid? deviceId = null) => new()
    {
        Email = email,
        Password = password,
        DeviceId = deviceId
    };
}