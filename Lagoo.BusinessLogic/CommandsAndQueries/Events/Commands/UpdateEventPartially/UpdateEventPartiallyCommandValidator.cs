using FluentValidation;
using Lagoo.BusinessLogic.CommandsAndQueries.Events.Commands.CreateEvent;
using Lagoo.BusinessLogic.Resources.CommandsAndQueries;

namespace Lagoo.BusinessLogic.CommandsAndQueries.Events.Commands.UpdateEventPartially;

public class UpdateEventPartiallyCommandValidator : AbstractValidator<UpdateEventPartiallyCommand>
{
    public UpdateEventPartiallyCommandValidator()
    {
        RuleFor(uepc => uepc.Id)
            .GreaterThan(0).WithMessage(EventResources.InvalidId);

        RuleFor(uepc => uepc.Name)
            .NotEmpty().WithMessage(EventResources.NameIsEmpty)
            .MaximumLength(CreateEventCommandValidator.NameMaxLength).WithMessage(
                string.Format(EventResources.NameIsTooLong, CreateEventCommandValidator.NameMaxLength))
            .When(uepc => uepc.Name is not null);

        RuleFor(uepc => uepc.Type)
            .IsInEnum().WithMessage(EventResources.InvalidType)
            .When(uepc => uepc.Type is not null);

        RuleFor(uepc => uepc.Address)
            .NotEmpty().WithMessage(EventResources.AddressIsEmpty)
            .MaximumLength(CreateEventCommandValidator.AddressMaxLength).WithMessage(
                string.Format(EventResources.AddressIsTooLong, CreateEventCommandValidator.AddressMaxLength))
            .When(uepc => uepc.Address is not null);

        RuleFor(uepc => uepc.Comment)
            .NotEmpty().WithMessage(EventResources.CommentIsEmpty)
            .MaximumLength(CreateEventCommandValidator.CommentMaxLength).WithMessage(
                string.Format(EventResources.CommentIsTooLong, CreateEventCommandValidator.CommentMaxLength))
            .When(uepc => uepc.Comment is not "" and not null);

        RuleFor(uepc => uepc.Duration)
            .NotEmpty().WithMessage(EventResources.DurationIsEmpty)
            .When(uepc => uepc.Duration is not null);

        RuleFor(uepc => uepc.BeginsAt)
            .NotEmpty().WithMessage(EventResources.BeginningDateIsEmpty)
            .When(uepc => uepc.BeginsAt is not null);
    }
}