using System.Reflection;
using AutoMapper;
using Lagoo.BusinessLogic.Common.AppOptions;
using Lagoo.BusinessLogic.Common.Extensions;
using Lagoo.BusinessLogic.Common.Mappings;
using Lagoo.BusinessLogic.Common.Services;
using Lagoo.BusinessLogic.Common.Services.HttpService;
using Lagoo.BusinessLogic.Common.UserAccessor;
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
}