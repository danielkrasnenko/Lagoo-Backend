namespace Lagoo.BusinessLogic.Common.Exceptions.Base;

/// <summary>
///   Exception gets thrown if one or more passed arguments is invalid.
/// </summary>
public class BaseArgumentException : BaseException
{
    public BaseArgumentException()
    {
    }

    public BaseArgumentException(string message) : base(message)
    {
    }
}