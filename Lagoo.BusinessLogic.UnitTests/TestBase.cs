using System;
using AutoMapper;
using Lagoo.BusinessLogic.Common.ExternalServices.Database;
using Lagoo.BusinessLogic.Common.Mappings;
using Lagoo.BusinessLogic.Common.Services.ExternalAuthServicesManager;
using Lagoo.BusinessLogic.Common.Services.HttpService;
using Lagoo.BusinessLogic.Common.UserAccessor;
using Lagoo.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace Lagoo.BusinessLogic.UnitTests;

public class TestBase
{
    protected static readonly Guid DefaultUserId = new("45987d62-19b0-440e-81ce-7049d71dedb2");

    protected IConfiguration Configuration = new ConfigurationBuilder().AddJsonFile("appsettings.Test.json").Build();

    protected IAppDbContext Context = Substitute.For<IAppDbContext>();

    protected IHttpService HttpService = Substitute.For<IHttpService>();

    protected IExternalAuthServicesManager ExternalAuthServicesManager = Substitute.For<IExternalAuthServicesManager>();

    protected IMediator Mediator = Substitute.For<IMediator>();

    protected EmptyConstraint IsNotNullOrEmpty = Is.Not.Null.And.Not.Empty;

    protected IMapper Mapper = CreateMapperSubstitution();

    protected UserManager<AppUser> UserManager = CreateUserManagerSubstitution();

    protected IUserAccessor UserAccessor = CreateUserAccessorSubstitution();

    protected AppUser DefaultUser = new AppUser
    {
        Id = DefaultUserId,
        UserName = "default-user@unit-tests.com",
        Email = "default-user@unit-tests.com",
        FirstName = "Paul",
        LastName = "Anderson"
    };

    private static IMapper CreateMapperSubstitution()
    {
        var configurationProvider = new MapperConfiguration(ce => ce.AddProfile<MappingProfile>());
        
        configurationProvider.AssertConfigurationIsValid();
        return configurationProvider.CreateMapper();
    }

    private static UserManager<AppUser> CreateUserManagerSubstitution()
    {
        var store = Substitute.For<IUserStore<AppUser>>();
        var userManager = Substitute.For<UserManager<AppUser>>(store, null, null, null, null, null, null, null, null);
        
        userManager.UserValidators.Add(new UserValidator<AppUser>());
        userManager.PasswordValidators.Add(new PasswordValidator<AppUser>());

        userManager.DeleteAsync(new AppUser()).ReturnsForAnyArgs(IdentityResult.Success);
        userManager.CreateAsync(new AppUser()).ReturnsForAnyArgs(IdentityResult.Success);
        userManager.UpdateAsync(new AppUser()).ReturnsForAnyArgs(IdentityResult.Success);
        userManager.AddToRolesAsync(new AppUser(), default).ReturnsForAnyArgs(IdentityResult.Success);
        userManager.AddLoginAsync(new AppUser(), default).ReturnsForAnyArgs(IdentityResult.Success);
        userManager.RemoveLoginAsync(new AppUser(), default, default).ReturnsForAnyArgs(IdentityResult.Success);

        return userManager;
    }

    private static IUserAccessor CreateUserAccessorSubstitution()
    {
        var userAccessor = Substitute.For<IUserAccessor>();
        userAccessor.UserId.Returns(DefaultUserId);
        userAccessor.IsAuthenticated.Returns(true);

        return userAccessor;
    }
}