using FluentValidation.Validators;
using Microsoft.Extensions.DependencyInjection;

namespace Lagoo.BusinessLogic.Common.Validators.Attributes;

/// <summary>
///   Life time of custom <see cref="PropertyValidator"/>
/// </summary>
public class ValidatorLifetimeAttribute : Attribute
{
    public ValidatorLifetimeAttribute(ServiceLifetime lifetime)
    {
        Lifetime = lifetime;
    }

    public ServiceLifetime Lifetime { get; set; }
}