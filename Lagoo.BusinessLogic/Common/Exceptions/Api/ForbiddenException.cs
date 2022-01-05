using Lagoo.BusinessLogic.Common.Exceptions.Base;

namespace Lagoo.BusinessLogic.Common.Exceptions.Api;

/// <summary>
///   An Exception gets thrown from a middleware with 403 status code
/// </summary>
public class ForbiddenException : BaseException
{
    private const string DefaultErrorMessage = "Access denied";
    
    public ForbiddenException() : base(DefaultErrorMessage)
    {
        Errors = new Dictionary<string, string[]>();
    }

    public ForbiddenException(params string[] errors) : this()
    {
        Errors.Add(string.Empty, errors);
    }
    
    public IDictionary<string, string[]> Errors { get; }
}