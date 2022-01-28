using FluentValidation;
using Lagoo.BusinessLogic.Resources.CommandsAndQueries;
using Lagoo.Domain.ConfigurationConstants;

namespace Lagoo.BusinessLogic.CommandsAndQueries.Events.Commands.UpdateEventPartially;

public class UpdateEventPartiallyCommandValidator : AbstractValidator<UpdateEventPartiallyCommand>
{
    public UpdateEventPartiallyCommandValidator()
    {
        RuleFor(uepc => uepc.Id)
            .GreaterThan(0).WithMessage(EventResources.InvalidId);

        RuleFor(uepc => uepc.Name)
            .NotEmpty().WithMessage(EventResources.NameIsEmpty)
            .MaximumLength(EventConfigurationConstansts.NameMaxLength).WithMessage(
                string.Format(EventResources.NameIsTooLong, EventConfigurationConstansts.NameMaxLength))
            .When(uepc => uepc.Name is not null);

        RuleFor(uepc => uepc.Type)
            .IsInEnum().WithMessage(EventResources.InvalidType)
            .When(uepc => uepc.Type is not null);

        RuleFor(uepc => uepc.Address)
            .NotEmpty().WithMessage(EventResources.AddressIsEmpty)
            .MaximumLength(EventConfigurationConstansts.AddressMaxLength).WithMessage(
                string.Format(EventResources.AddressIsTooLong, EventConfigurationConstansts.AddressMaxLength))
            .When(uepc => uepc.Address is not null);

        RuleFor(uepc => uepc.Comment)
            .NotEmpty().WithMessage(EventResources.CommentIsEmpty)
            .MaximumLength(EventConfigurationConstansts.CommentMaxLength).WithMessage(
                string.Format(EventResources.CommentIsTooLong, EventConfigurationConstansts.CommentMaxLength))
            .When(uepc => uepc.Comment is not "" and not null);

        RuleFor(uepc => uepc.Duration)
            .Must(d => d != TimeSpan.Zero).WithMessage(EventResources.DurationIsEmpty)
            .When(uepc => uepc.Duration is not null);

        RuleFor(uepc => uepc.BeginsAt)
            .Must(ba => ba > DateTime.UtcNow).WithMessage(EventResources.BeginningDateIsInvalid)
            .When(uepc => uepc.BeginsAt is not null);
    }
}