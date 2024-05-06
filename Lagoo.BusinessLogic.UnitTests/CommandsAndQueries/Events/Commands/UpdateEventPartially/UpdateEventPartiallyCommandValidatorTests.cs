using System;
using FluentValidation.Results;
using Lagoo.BusinessLogic.CommandsAndQueries.Events.Commands.UpdateEventPartially;
using Lagoo.BusinessLogic.UnitTests.Common.Base;
using Lagoo.BusinessLogic.UnitTests.Common.Helpers;
using Lagoo.Domain.ConfigurationConstants;
using Lagoo.Domain.Enums;
using NUnit.Framework;

namespace Lagoo.BusinessLogic.UnitTests.CommandsAndQueries.Events.Commands.UpdateEventPartially;

/// <summary>
///   Tests for <see cref="UpdateEventPartiallyCommandValidator"/>
/// </summary>
[TestFixture]
public class UpdateEventPartiallyCommandValidatorTests : TestsBase
{
    [Test]
    public void Validate_CommandContainsValidData_ShouldReturnValidResultOfValidation()
    {
        var result = PerformValidation(new UpdateEventPartiallyCommand
        {
            EventId = 1,
            Name = "Name",
            Type = EventType.Convention,
            Address = "Address",
            Comment = "Comment",
            Duration = TimeSpan.FromHours(1),
            IsPrivate = true,
            BeginsAt = DateTime.UtcNow.AddDays(2)
        });
        
        Assert.IsTrue(result.IsValid);
    }
    
    [Test]
    public void Validate_CommandWithInvalidId_ShouldReturnInvalidResultOfValidation()
    {
        var result = PerformValidation(new UpdateEventPartiallyCommand { EventId = -1 });
        
        Assert.IsFalse(result.IsValid);
    }
    
    [TestCase("")]
    [TestCase("   ")]
    public void Validate_CommandWithEmptyOrWhitespaceName_ShouldReturnInvalidResultOfValidation(string name)
    {
        var result = PerformValidation(new UpdateEventPartiallyCommand { EventId = 1, Name = name });
        
        Assert.IsFalse(result.IsValid);
    }

    [Test]
    public void Validate_CommandWithTooLongName_ShouldReturnInvalidResultOfValidation()
    {
        var longName = StringHelpers.GenerateRandomString(EventConfigurationConstansts.NameMaxLength + 1);

        var result = PerformValidation(new UpdateEventPartiallyCommand { EventId = 1, Name = longName });
        
        Assert.IsFalse(result.IsValid);
    }

    [Test]
    public void Validate_CommandWithInvalidType_ShouldReturnInvalidResultOfValidation()
    {
        var result = PerformValidation(new UpdateEventPartiallyCommand { EventId =  1, Type = (EventType) 100 });
        
        Assert.IsFalse(result.IsValid);
    }
    
    [TestCase("")]
    [TestCase("   ")]
    public void Validate_CommandWithEmptyOrWhitespaceAddress_ShouldReturnInvalidResultOfValidation(string address)
    {
        var result = PerformValidation(new UpdateEventPartiallyCommand { EventId = 1, Address = address });
        
        Assert.IsFalse(result.IsValid);
    }

    [Test]
    public void Validate_CommandWithTooLongAddress_ShouldReturnInvalidResultOfValidation()
    {
        var longAddress = StringHelpers.GenerateRandomString(EventConfigurationConstansts.AddressMaxLength + 1);

        var result = PerformValidation(new UpdateEventPartiallyCommand { EventId = 1, Address = longAddress });
        
        Assert.IsFalse(result.IsValid);
    }
    
    [Test]
    public void Validate_CommandWithWhitespaceComment_ShouldReturnInvalidResultOfValidation()
    {
        var result = PerformValidation(new UpdateEventPartiallyCommand { EventId = 1, Comment = "   " });
        
        Assert.IsFalse(result.IsValid);
    }

    [Test]
    public void Validate_CommandWithTooLongComment_ShouldReturnInvalidResultOfValidation()
    {
        var longComment = StringHelpers.GenerateRandomString(EventConfigurationConstansts.CommentMaxLength + 1);

        var result = PerformValidation(new UpdateEventPartiallyCommand { EventId = 1, Comment = longComment });
        
        Assert.IsFalse(result.IsValid);
    }

    [Test]
    public void Validate_CommandWithDefaultDuration_ShouldReturnInvalidResultOfValidation()
    {
        var result = PerformValidation(new UpdateEventPartiallyCommand { EventId = 1, Duration = TimeSpan.Zero });
        
        Assert.IsFalse(result.IsValid);
    }

    [Test]
    public void Validate_CommandWithPastBeginningDate_ShouldReturnInvalidResultOfValidation()
    {
        var result = PerformValidation(new UpdateEventPartiallyCommand { EventId = 1, BeginsAt = DateTime.UtcNow.AddMonths(-1) });
        
        Assert.IsFalse(result.IsValid);
    }
    
    private UpdateEventPartiallyCommandValidator CreateValidator() => new();

    private ValidationResult PerformValidation(UpdateEventPartiallyCommand command)
    {
        var validator = CreateValidator();

        return validator.Validate(command);
    }
}