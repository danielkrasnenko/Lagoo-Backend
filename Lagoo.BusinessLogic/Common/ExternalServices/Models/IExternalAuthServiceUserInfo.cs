namespace Lagoo.BusinessLogic.Common.ExternalServices.Models;

/// <summary>
///   User information from external auth services
/// </summary>
public interface IExternalAuthServiceUserInfo
{
    public string Id { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Email { get; set; }
}