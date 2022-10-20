using Lagoo.BusinessLogic.Common.Mappings;
using Lagoo.BusinessLogic.Common.Services.ExternalAuthServicesManager;

namespace Lagoo.BusinessLogic.CommandsAndQueries.Accounts.Queries.GetExternalAuthServiceUserInfo;

public class GetExternalAuthServiceUserInfoResponseDto : IMapFrom<IExternalAuthServiceUserInfo>
{
    public string Id { get; set; } = string.Empty;
    
    public string FirstName { get; set; } = string.Empty;
    
    public string LastName { get; set; } = string.Empty;
    
    public string Email { get; set; } = string.Empty;
}