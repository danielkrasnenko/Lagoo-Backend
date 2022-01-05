using Lagoo.BusinessLogic.Common.Exceptions.Base;

namespace Lagoo.BusinessLogic.Common.Exceptions.Api;

/// <summary>
///   An Exception gets thrown from a middleware with 404 status code
/// </summary>
public class NotFoundException : BaseException
{
    private const string DefaultErrorMessage = "Requested resources were not found";
    
    public NotFoundException() : base(DefaultErrorMessage)
    {
        Errors = new Dictionary<string, string[]>();
    }

    public NotFoundException(params string[] errors) : this()
    {
        Errors.Add(string.Empty, errors);
    }

    public IDictionary<string, string[]> Errors { get; }
}