using System;
using AutoMapper;
using Lagoo.BusinessLogic.Common.Mappings;
using Lagoo.BusinessLogic.Common.Services.ExternalAuthServicesManager;
using Lagoo.BusinessLogic.Common.Services.HttpService;
using Lagoo.BusinessLogic.Common.Services.UserAccessor;
using Lagoo.BusinessLogic.Core.Repositories;
using Lagoo.Domain.Entities;
using Lagoo.Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace Lagoo.BusinessLogic.UnitTests.Common.Base;

/// <summary>
///   Base class for all tests which contains mocks of main service
/// </summary>
public class TestsBase
{
    protected static readonly Guid DefaultUserId = Guid.NewGuid();

    protected const string DefaultUserEmail = "default-user@unit-tests.com";

    protected readonly IConfiguration Configuration = new ConfigurationBuilder()
        .AddJsonFile(JsonConfigFileName)
        .AddUserSecrets(UserSecretsId)
        .Build();

    protected readonly AppDbContext Context = Substitute.For<AppDbContext>();
    
    protected IUserRepository UserRepository = Substitute.For<IUserRepository>();

    protected IRefreshTokenRepository RefreshTokenRepository = Substitute.For<IRefreshTokenRepository>();
    
    protected IEventRepository EventRepository = Substitute.For<IEventRepository>();

    protected IHttpService HttpService = Substitute.For<IHttpService>();

    protected IExternalAuthServicesManager ExternalAuthServicesManager = Substitute.For<IExternalAuthServicesManager>();

    protected IMediator Mediator = Substitute.For<IMediator>();

    protected IMapper Mapper = CreateMapperSubstitution();

    protected UserManager<AppUser> UserManager = CreateUserManagerSubstitution();

    protected IUserAccessor UserAccessorOfDefaultUser = CreateUserAccessorOfDefaultUserSubstitution();

    protected static readonly AppUser DefaultUser = new()
    {
        Id = DefaultUserId,
        UserName = DefaultUserEmail,
        Email = DefaultUserEmail,
        FirstName = "Paul",
        LastName = "Anderson"
    };
    
    protected readonly EmptyConstraint IsNotNullOrEmpty = Is.Not.Null.And.Not.Empty;
    
    private const string JsonConfigFileName = "appsettings.Test.json";
    
    private const string UserSecretsId = "4bd89ed4-423f-4131-bb3c-983b6dc107d2";

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

    private static IUserAccessor CreateUserAccessorOfDefaultUserSubstitution()
    {
        var userAccessor = Substitute.For<IUserAccessor>();
        userAccessor.UserId.Returns(DefaultUserId);
        userAccessor.IsAuthenticated.Returns(true);

        return userAccessor;
    }
}