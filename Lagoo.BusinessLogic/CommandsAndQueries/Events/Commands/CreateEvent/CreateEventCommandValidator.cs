using FluentValidation;
using Lagoo.BusinessLogic.Resources.CommandsAndQueries;

namespace Lagoo.BusinessLogic.CommandsAndQueries.Events.Commands.CreateEvent;

public class CreateEventCommandValidator : AbstractValidator<CreateEventCommand>
{
    private const int NameMaxLength = 256;

    private const int AddressMaxLength = 512;

    private const int CommentMaxLength = 1026;
    
    public CreateEventCommandValidator()
    {
        RuleFor(cec => cec.Name)
            .NotEmpty().WithMessage(EventResources.NameIsEmpty)
            .MaximumLength(NameMaxLength).WithMessage(string.Format(EventResources.NameIsTooLong, NameMaxLength));

        RuleFor(cec => cec.Type)
            .IsInEnum().WithMessage(EventResources.InvalidType);

        RuleFor(cec => cec.Address)
            .NotEmpty().WithMessage(EventResources.AddressIsEmpty)
            .MaximumLength(AddressMaxLength).WithMessage(string.Format(EventResources.AddressIsTooLong, AddressMaxLength));

        RuleFor(cec => cec.Comment)
            .NotEmpty().WithMessage(EventResources.CommentIsEmpty)
            .MaximumLength(CommentMaxLength).WithMessage(string.Format(EventResources.CommentIsTooLong, CommentMaxLength))
            .When(cec => cec.Comment is not null);

        RuleFor(cec => cec.Duration)
            .NotEmpty().WithMessage(EventResources.DurationIsEmpty);

        RuleFor(cec => cec.BeginsAt)
            .NotEmpty().WithMessage(EventResources.BeginningDateIsEmpty);
    }
}