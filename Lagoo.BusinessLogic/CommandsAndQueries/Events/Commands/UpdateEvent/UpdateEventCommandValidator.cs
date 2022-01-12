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

        RuleFor(uec => uec.Name)
            .NotEmpty().WithMessage(EventResources.NameIsEmpty)
            .MaximumLength(CreateEventCommandValidator.NameMaxLength).WithMessage(
                string.Format(EventResources.NameIsTooLong, CreateEventCommandValidator.NameMaxLength));

        RuleFor(uec => uec.Type)
            .IsInEnum().WithMessage(EventResources.InvalidType);

        RuleFor(uec => uec.Address)
            .NotEmpty().WithMessage(EventResources.AddressIsEmpty)
            .MaximumLength(CreateEventCommandValidator.AddressMaxLength).WithMessage(
                string.Format(EventResources.AddressIsTooLong, CreateEventCommandValidator.AddressMaxLength));

        RuleFor(uec => uec.Comment)
            .NotEmpty().WithMessage(EventResources.CommentIsEmpty)
            .MaximumLength(CreateEventCommandValidator.CommentMaxLength).WithMessage(
                string.Format(EventResources.CommentIsTooLong, CreateEventCommandValidator.CommentMaxLength))
            .When(uec => uec.Comment is not "");

        RuleFor(uec => uec.Duration)
            .NotEmpty().WithMessage(EventResources.DurationIsEmpty);

        RuleFor(uec => uec.BeginsAt)
            .NotEmpty().WithMessage(EventResources.BeginningDateIsEmpty);
    }
}