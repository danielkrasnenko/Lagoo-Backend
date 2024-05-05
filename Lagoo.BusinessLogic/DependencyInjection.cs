using System.Reflection;
using AutoMapper;
using Lagoo.BusinessLogic.Common.Mappings;
using MediatR;
using MediatR.Extensions.FluentValidation.AspNetCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Lagoo.BusinessLogic;

/// <summary>
///   Adding all business logic services to DI Container
/// </summary>
public static class DependencyInjection
{
    public static void AddBusinessLogic(this IServiceCollection services)
    {
        services.AddSingleton(GenerateMapper());

        var currentAssembly = Assembly.GetExecutingAssembly();

        services.AddMediatR(c => c.RegisterServicesFromAssembly(currentAssembly));
        services.AddFluentValidation(new[] { currentAssembly });

        services.AddHttpContextAccessor();
    }

    private static IMapper GenerateMapper()
    {
        var mapperConfiguration = new MapperConfiguration(mc => mc.AddProfile(new MappingProfile()));
        mapperConfiguration.AssertConfigurationIsValid();

        return mapperConfiguration.CreateMapper();
    }
}