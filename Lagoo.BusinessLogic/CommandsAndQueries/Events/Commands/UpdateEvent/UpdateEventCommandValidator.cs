using FluentValidation;
using Lagoo.BusinessLogic.CommandsAndQueries.Events.Commands.CreateEvent;
using Lagoo.BusinessLogic.Resources.CommandsAndQueries;

namespace Lagoo.BusinessLogic.CommandsAndQueries.Events.Commands.UpdateEvent;

public class UpdateEventCommandValidator : AbstractValidator<UpdateEventCommand>
{
    public UpdateEventCommandValidator()
    {
        RuleFor(uec => uec.Id)
            .GreaterThan(0).WithMessage(EventResources.InvalidId);
        
        RuleFor(cec => cec.Name)
            .NotEmpty().WithMessage(EventResources.NameIsEmpty)
            .MaximumLength(CreateEventCommandValidator.NameMaxLength).WithMessage(string.Format(EventResources.NameIsTooLong, CreateEventCommandValidator.NameMaxLength));

        RuleFor(cec => cec.Type)
            .IsInEnum().WithMessage(EventResources.InvalidType);

        RuleFor(cec => cec.Address)
            .NotEmpty().WithMessage(EventResources.AddressIsEmpty)
            .MaximumLength(CreateEventCommandValidator.AddressMaxLength).WithMessage(string.Format(EventResources.AddressIsTooLong, CreateEventCommandValidator.AddressMaxLength));

        RuleFor(cec => cec.Comment)
            .NotEmpty().WithMessage(EventResources.CommentIsEmpty)
            .MaximumLength(CreateEventCommandValidator.CommentMaxLength).WithMessage(string.Format(EventResources.CommentIsTooLong, CreateEventCommandValidator.CommentMaxLength))
            .When(cec => cec.Comment is not "");

        RuleFor(cec => cec.Duration)
            .NotEmpty().WithMessage(EventResources.DurationIsEmpty);

        RuleFor(cec => cec.BeginsAt)
            .NotEmpty().WithMessage(EventResources.BeginningDateIsEmpty);
    }
}