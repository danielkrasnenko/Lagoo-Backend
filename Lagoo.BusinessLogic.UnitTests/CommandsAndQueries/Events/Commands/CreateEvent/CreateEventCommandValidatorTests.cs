using System;
using System.Linq;
using Lagoo.BusinessLogic.CommandsAndQueries.Events.Commands.CreateEvent;
using Lagoo.BusinessLogic.UnitTests.Common.Base;
using Lagoo.Domain.Enums;
using NUnit.Framework;

namespace Lagoo.BusinessLogic.UnitTests.CommandsAndQueries.Events.Commands.CreateEvent;

[TestFixture]
public class CreateEventCommandValidatorTests : TestBase
{
    private readonly Random _random = new();
    
    [Test]
    public void Validate_ProvidedDataIsValid_ShouldReturnValidResultOfValidation()
    {
        var command = GenerateCommandWithValidDefaultData();

        var validator = CreateValidator();

        var result = validator.Validate(command);
        
        Assert.IsTrue(result.IsValid);
    }
    
    [TestCase(null)]
    [TestCase("")]
    public void Validate_CommandWithOmittedOrEmptyName_ShouldReturnInvalidResultOfValidation(string? name)
    {
        var command = GenerateCommandWithValidDefaultData(name);

        var validator = CreateValidator();

        var result = validator.Validate(command);
        
        Assert.IsFalse(result.IsValid);
    }

    [Test]
    public void Validate_CommandWithTooLongName_ShouldReturnInvalidResultOfValidation()
    {
        var longName = GenerateRandomString(500);
        
        var command = GenerateCommandWithValidDefaultData(longName);

        var validator = CreateValidator();

        var result = validator.Validate(command);
        
        Assert.IsFalse(result.IsValid);
    }

    [Test]
    public void Validate_CommandWithInvalidType_ShouldReturnInvalidResultOfValidation()
    {
        var command = GenerateCommandWithValidDefaultData(type: (EventType) 100);

        var validator = CreateValidator();

        var result = validator.Validate(command);
        
        Assert.IsFalse(result.IsValid);
    }
    
    [TestCase(null)]
    [TestCase("")]
    public void Validate_CommandWithNullableOrEmptyAddress_ShouldReturnInvalidResultOfValidation(string? address)
    {
        var command = GenerateCommandWithValidDefaultData(address);

        var validator = CreateValidator();

        var result = validator.Validate(command);
        
        Assert.IsFalse(result.IsValid);
    }

    [Test]
    public void Validate_CommandWithTooLongAddress_ShouldReturnInvalidResultOfValidation()
    {
        var longAddress = GenerateRandomString(1000);
        
        var command = GenerateCommandWithValidDefaultData(address: longAddress);

        var validator = CreateValidator();

        var result = validator.Validate(command);
        
        Assert.IsFalse(result.IsValid);
    }
    
    [Test]
    public void Validate_CommandWithNullableComment_ShouldReturnInvalidResultOfValidation()
    {
        var command = GenerateCommandWithValidDefaultData(comment: null);

        var validator = CreateValidator();

        var result = validator.Validate(command);
        
        Assert.IsFalse(result.IsValid);
    }
    
    [Test]
    public void Validate_CommandWithTooLongComment_ShouldReturnInvalidResultOfValidation()
    {
        var longComment = GenerateRandomString(2000);
        
        var command = GenerateCommandWithValidDefaultData(comment: longComment);

        var validator = CreateValidator();
        
        var result = validator.Validate(command);
        
        Assert.IsFalse(result.IsValid);
    }

    [Test]
    public void Validate_CommandWithDefaultDuration_ShouldReturnInvalidResultOfValidation()
    {
        var command = GenerateCommandWithValidDefaultData(duration: new TimeSpan());

        var validator = CreateValidator();
        
        var result = validator.Validate(command);
        
        Assert.IsFalse(result.IsValid);
    }

    [Test]
    public void Validate_CommandWithDefaultBeginningDate_ShouldReturnInvalidResultOfValidation()
    {
        var command = GenerateCommandWithValidDefaultData(beginsAt: new DateTime());

        var validator = CreateValidator();
        
        var result = validator.Validate(command);
        
        Assert.IsFalse(result.IsValid);
    }

    private CreateEventCommandValidator CreateValidator() => new();

    private CreateEventCommand GenerateCommandWithValidDefaultData(string name = "Name", EventType type = EventType.Ceremony,
        string address = "Long street 19", string comment = "Nice", bool isPrivate = false, TimeSpan? duration = null, DateTime? beginsAt = null
    ) => new()
    {
        Name = name,
        Type = type,
        Address = address,
        Comment = comment,
        Duration = duration ?? TimeSpan.FromHours(1),
        BeginsAt = beginsAt ?? DateTime.UtcNow,
        IsPrivate = isPrivate
    };

    private string GenerateRandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length).Select(s => s[_random.Next(s.Length)]).ToArray());
    }
}