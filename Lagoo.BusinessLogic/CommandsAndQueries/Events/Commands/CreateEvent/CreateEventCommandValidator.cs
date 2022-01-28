using FluentValidation;
using Lagoo.BusinessLogic.Resources.CommandsAndQueries;
using Lagoo.Domain.ConfigurationConstants;

namespace Lagoo.BusinessLogic.CommandsAndQueries.Events.Commands.CreateEvent;

public class CreateEventCommandValidator : AbstractValidator<CreateEventCommand>
{
    public CreateEventCommandValidator()
    {
        RuleFor(cec => cec.Name)
            .NotEmpty().WithMessage(EventResources.NameIsEmpty)
            .MaximumLength(EventConfigurationConstansts.NameMaxLength).WithMessage(string.Format(EventResources.NameIsTooLong, EventConfigurationConstansts.NameMaxLength));

        RuleFor(cec => cec.Type)
            .IsInEnum().WithMessage(EventResources.InvalidType);

        RuleFor(cec => cec.Address)
            .NotEmpty().WithMessage(EventResources.AddressIsEmpty)
            .MaximumLength(EventConfigurationConstansts.AddressMaxLength).WithMessage(string.Format(EventResources.AddressIsTooLong, EventConfigurationConstansts.AddressMaxLength));

        RuleFor(cec => cec.Comment)
            .NotEmpty().WithMessage(EventResources.CommentIsEmpty)
            .MaximumLength(EventConfigurationConstansts.CommentMaxLength).WithMessage(string.Format(EventResources.CommentIsTooLong, EventConfigurationConstansts.CommentMaxLength))
            .When(cec => cec.Comment is not "");

        RuleFor(cec => cec.Duration)
            .NotEmpty().WithMessage(EventResources.DurationIsEmpty);

        RuleFor(cec => cec.BeginsAt)
            .Must(ba => ba > DateTime.UtcNow).WithMessage(EventResources.BeginningDateIsInvalid);
    }
}