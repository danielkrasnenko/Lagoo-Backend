using System.Reflection;
using AutoMapper;
using Lagoo.BusinessLogic.Common.Extensions;
using Lagoo.BusinessLogic.Common.Mappings;
using Lagoo.BusinessLogic.Common.Services.HttpService;
using MediatR;
using MediatR.Extensions.FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;

namespace Lagoo.BusinessLogic;

public static class DependencyInjection
{
    public static void AddBusinessLogic(this IServiceCollection services)
    {
        services.AddSingleton(GenerateMapper());

        var currentAssembly = Assembly.GetExecutingAssembly();

        services.AddMediatR(currentAssembly);
        services.AddFluentValidation(new []{ currentAssembly });

        services.AddHttpContextAccessor();

        services.AddHttpClient<IHttpService, HttpService>();

        services.AddPropertyValidatorsFromAssembly();
    }

    private static IMapper GenerateMapper()
    {
        var mapperConfiguration = new MapperConfiguration(mc => mc.AddProfile(new MappingProfile()));
        mapperConfiguration.AssertConfigurationIsValid();

        return mapperConfiguration.CreateMapper();
    }
}