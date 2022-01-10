using System.ComponentModel;

namespace Lagoo.Domain.Enums;

/// <summary>
///   Different external authentication services
/// </summary>
public enum ExternalAuthService : byte
{
    [Description("Facebook")]
    Facebook = 1,
    
    [Description("Google")]
    Google = 2
}