using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Lagoo.BusinessLogic.Common.UserAccessor;

/// <summary>
///  Accessor for retrieving User information from JWT
/// </summary>
public class UserAccessor : IUserAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserAccessor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException();
        
        UserId = RetrieveUserId();
    }
    
    public Guid? UserId { get; }
    
    public string? Role => ClaimsPrincipal?.FindFirst(c => c.Type == ClaimTypes.Role)?.Value ?? null;
    
    public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;
    
    public ClaimsPrincipal? ClaimsPrincipal => _httpContextAccessor.HttpContext?.User;

    private Guid? RetrieveUserId()
    {
        var retrievedStringId = ClaimsPrincipal?.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        
        if (retrievedStringId is null)
        {
            return null;
        }

        return Guid.Parse(retrievedStringId);
    }
}