using System;
using FluentValidation.Results;
using Lagoo.BusinessLogic.CommandsAndQueries.Events.Commands.CreateEvent;
using Lagoo.BusinessLogic.UnitTests.Common.Base;
using Lagoo.BusinessLogic.UnitTests.Common.Helpers;
using Lagoo.Domain.Enums;
using NUnit.Framework;

namespace Lagoo.BusinessLogic.UnitTests.CommandsAndQueries.Events.Commands.CreateEvent;

/// <summary>
///   Tests for <see cref="CreateEventCommandValidator"/>
/// </summary>
[TestFixture]
public class CreateEventCommandValidatorTests : TestsBase
{
    [Test]
    public void Validate_ProvidedDataIsValid_ShouldReturnValidResultOfValidation()
    {
        var result = PerformValidation(GenerateCommandWithValidDefaultData());
        
        Assert.IsTrue(result.IsValid);
    }
    
    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    public void Validate_CommandWithOmittedOrEmptyOrWhitespaceName_ShouldReturnInvalidResultOfValidation(string? name)
    {
        var result = PerformValidation(GenerateCommandWithValidDefaultData(name));
        
        Assert.IsFalse(result.IsValid);
    }

    [Test]
    public void Validate_CommandWithTooLongName_ShouldReturnInvalidResultOfValidation()
    {
        var longName = StringHelpers.GenerateRandomString(500);

        var result = PerformValidation(GenerateCommandWithValidDefaultData(longName));
        
        Assert.IsFalse(result.IsValid);
    }

    [Test]
    public void Validate_CommandWithInvalidType_ShouldReturnInvalidResultOfValidation()
    {
        var result = PerformValidation(GenerateCommandWithValidDefaultData(type: (EventType) 100));
        
        Assert.IsFalse(result.IsValid);
    }
    
    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    public void Validate_CommandWithOmittedOrEmptyOrWhitespaceAddress_ShouldReturnInvalidResultOfValidation(string? address)
    {
        var result = PerformValidation(GenerateCommandWithValidDefaultData(address));
        
        Assert.IsFalse(result.IsValid);
    }

    [Test]
    public void Validate_CommandWithTooLongAddress_ShouldReturnInvalidResultOfValidation()
    {
        var longAddress = StringHelpers.GenerateRandomString(1000);

        var result = PerformValidation(GenerateCommandWithValidDefaultData(address: longAddress));
        
        Assert.IsFalse(result.IsValid);
    }
    
    [TestCase(null)]
    [TestCase("   ")]
    public void Validate_CommandWithOmittedOrWhitespaceComment_ShouldReturnInvalidResultOfValidation(string? comment)
    {
        var result = PerformValidation(GenerateCommandWithValidDefaultData(comment: comment));
        
        Assert.IsFalse(result.IsValid);
    }
    
    [Test]
    public void Validate_CommandWithTooLongComment_ShouldReturnInvalidResultOfValidation()
    {
        var longComment = StringHelpers.GenerateRandomString(2000);

        var result = PerformValidation(GenerateCommandWithValidDefaultData(comment: longComment));
        
        Assert.IsFalse(result.IsValid);
    }

    [Test]
    public void Validate_CommandWithDefaultDuration_ShouldReturnInvalidResultOfValidation()
    {
        var result = PerformValidation(GenerateCommandWithValidDefaultData(duration: TimeSpan.Zero));
        
        Assert.IsFalse(result.IsValid);
    }

    [Test]
    public void Validate_CommandWithPastBeginningDate_ShouldReturnInvalidResultOfValidation()
    {
        var result = PerformValidation(GenerateCommandWithValidDefaultData(beginsAt: DateTime.UtcNow.AddMonths(-1)));
        
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

    private ValidationResult PerformValidation(CreateEventCommand command)
    {
        var validator = CreateValidator();

        return validator.Validate(command);
    }
}