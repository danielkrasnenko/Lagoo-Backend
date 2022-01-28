using FluentValidation;
using Lagoo.BusinessLogic.Resources.CommandsAndQueries;
using Lagoo.Domain.ConfigurationConstants;

namespace Lagoo.BusinessLogic.CommandsAndQueries.Events.Commands.UpdateEvent;

public class UpdateEventCommandValidator : AbstractValidator<UpdateEventCommand>
{
    public UpdateEventCommandValidator()
    {
        RuleFor(uec => uec.Id)
            .GreaterThan(0).WithMessage(EventResources.InvalidId);

        RuleFor(uec => uec.Name)
            .NotEmpty().WithMessage(EventResources.NameIsEmpty)
            .MaximumLength(EventConfigurationConstansts.NameMaxLength).WithMessage(
                string.Format(EventResources.NameIsTooLong, EventConfigurationConstansts.NameMaxLength));

        RuleFor(uec => uec.Type)
            .IsInEnum().WithMessage(EventResources.InvalidType);

        RuleFor(uec => uec.Address)
            .NotEmpty().WithMessage(EventResources.AddressIsEmpty)
            .MaximumLength(EventConfigurationConstansts.AddressMaxLength).WithMessage(
                string.Format(EventResources.AddressIsTooLong, EventConfigurationConstansts.AddressMaxLength));

        RuleFor(uec => uec.Comment)
            .NotEmpty().WithMessage(EventResources.CommentIsEmpty)
            .MaximumLength(EventConfigurationConstansts.CommentMaxLength).WithMessage(
                string.Format(EventResources.CommentIsTooLong, EventConfigurationConstansts.CommentMaxLength))
            .When(uec => uec.Comment is not "");

        RuleFor(uec => uec.Duration)
            .NotEmpty().WithMessage(EventResources.DurationIsEmpty);

        RuleFor(uec => uec.BeginsAt)
            .Must(ba => ba > DateTime.UtcNow).WithMessage(EventResources.BeginningDateIsInvalid);
    }
}