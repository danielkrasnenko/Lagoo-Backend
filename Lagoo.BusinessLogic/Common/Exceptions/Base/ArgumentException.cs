namespace Lagoo.BusinessLogic.Common.Exceptions.Base;

/// <summary>
///  Exception gets thrown if one or more passed arguments is invalid.
/// </summary>
public class ArgumentException : BaseException
{
    public ArgumentException()
    {
    }

    public ArgumentException(string message) : base(message)
    {
    }
}