using Microsoft.AspNetCore.Identity;

namespace Lagoo.Domain.Entities;

/// <summary>
///  A model of the application user
/// </summary>
public class AppUser : IdentityUser<Guid>
{
    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;
    
    public string? Address { get; set; }
    
    public DateTimeOffset RegistrationUtcDate { get; set; }
}