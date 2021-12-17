using System.Reflection;
using FluentValidation.Validators;
using Lagoo.BusinessLogic.Common.Validators.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace Lagoo.BusinessLogic.Common.Extensions;

/// <summary>
///  Extension methods for <see cref="IServiceCollection"/>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    ///  Add property validators to the app
    /// </summary>
    /// <param name="services">Service collection</param>
    public static void AddPropertyValidatorsFromAssembly(this IServiceCollection services)
    {
        var validators = Assembly.GetExecutingAssembly().GetExportedTypes()
            .Where(t => t.IsSubclassOf(typeof(PropertyValidator<,>)));

        foreach (var validator in validators)
        {
            var lifetime = Attribute.GetCustomAttribute(validator, typeof(ValidatorLifetimeAttribute)) as ValidatorLifetimeAttribute;
            
            if (lifetime is not null)
            {
                services.Add(new ServiceDescriptor(validator, validator, lifetime.Lifetime));
            }
        }
    }
}