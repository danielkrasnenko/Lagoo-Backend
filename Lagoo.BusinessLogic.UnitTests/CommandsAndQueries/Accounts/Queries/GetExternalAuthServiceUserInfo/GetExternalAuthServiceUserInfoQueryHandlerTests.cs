using System.Threading;
using System.Threading.Tasks;
using Lagoo.BusinessLogic.CommandsAndQueries.Accounts.Queries.GetExternalAuthServiceUserInfo;
using Lagoo.BusinessLogic.UnitTests.CommandsAndQueries.Accounts.Common;
using Lagoo.Domain.Enums;
using NUnit.Framework;

namespace Lagoo.BusinessLogic.UnitTests.CommandsAndQueries.Accounts.Queries.GetExternalAuthServiceUserInfo;

/// <summary>
///   Tests for <see cref="GetExternalAuthServiceUserInfoQueryHandler"/>
/// </summary>
[TestFixture]
public class GetExternalAuthServiceUserInfoQueryHandlerTests : AccountTestsBase
{
    [Test]
    public async Task Handle_QueryHasAllNeededParams_ShouldReturnUserInfo()
    {
        var query = new GetExternalAuthServiceUserInfoQuery
        {
            ExternalAuthService = ExternalAuthService.Facebook,
            ExternalAuthServiceAccessToken = "access-token"
        };

        var handler = new GetExternalAuthServiceUserInfoQueryHandler(ExternalAuthServicesManager, Mapper);

        var result = await handler.Handle(query, CancellationToken.None);
        
        Assert.AreEqual(ExternalAuthServiceDefaultUserId, result.Id);
        Assert.AreEqual(DefaultUserEmail, result.Email);
        Assert.AreEqual(DefaultUser.FirstName, result.FirstName);
        Assert.AreEqual(DefaultUser.LastName, result.LastName);
    }
}