using System.Reflection;
using AutoMapper;
using FluentValidation.Validators;
using Lagoo.BusinessLogic.Common.AppOptions;
using Lagoo.BusinessLogic.Common.Extensions;
using Lagoo.BusinessLogic.Common.Mappings;
using Lagoo.BusinessLogic.Common.Services;
using Lagoo.BusinessLogic.Common.Services.HttpService;
using Lagoo.BusinessLogic.Common.UserAccessor;
using Lagoo.BusinessLogic.Common.Validators.Attributes;
using MediatR;
using MediatR.Extensions.FluentValidation.AspNetCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Lagoo.BusinessLogic;

/// <summary>
///  Adding all business logic services to DI Container
/// </summary>
public static class DependencyInjection
{
    public static void AddBusinessLogic(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAppOptions(configuration);

        services.AddSingleton(GenerateMapper());

        var currentAssembly = Assembly.GetExecutingAssembly();

        services.AddMediatR(currentAssembly);
        services.AddFluentValidation(new []{ currentAssembly });

        services.AddHttpContextAccessor();

        services.AddScoped<IUserAccessor, UserAccessor>();

        services.AddPropertyValidatorsFromAssembly();
        
        services.AddBusinessLogicServices();
    }

    private static IMapper GenerateMapper()
    {
        var mapperConfiguration = new MapperConfiguration(mc => mc.AddProfile(new MappingProfile()));
        mapperConfiguration.AssertConfigurationIsValid();

        return mapperConfiguration.CreateMapper();
    }
    
    /// <summary>
    ///  Add property validators to the app
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/></param>
    private static void AddPropertyValidatorsFromAssembly(this IServiceCollection services)
    {
        var validators = Assembly.GetExecutingAssembly().GetExportedTypes()
            .Where(t => t.IsSubclassOf(typeof(PropertyValidator<,>)));

        foreach (var validator in validators)
        {
            if (Attribute.GetCustomAttribute(validator, typeof(ValidatorLifetimeAttribute)) is ValidatorLifetimeAttribute lifetime)
            {
                services.Add(new ServiceDescriptor(validator, validator, lifetime.Lifetime));
            }
        }
    }
}