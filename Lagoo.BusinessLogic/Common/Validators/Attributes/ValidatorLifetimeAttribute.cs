using Microsoft.Extensions.DependencyInjection;

namespace Lagoo.BusinessLogic.Common.Validators.Attributes;

public class ValidatorLifetimeAttribute : Attribute
{
    public ValidatorLifetimeAttribute(ServiceLifetime lifetime)
    {
        Lifetime = lifetime;
    }

    public ServiceLifetime Lifetime { get; set; }
}