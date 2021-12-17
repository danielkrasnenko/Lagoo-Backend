namespace Lagoo.BusinessLogic.Common.Exceptions.Base;

/// <summary>
///  Represents a base class for exceptions, like a wrapper over <see cref="Exception"/>
/// </summary>
/// <remarks>End user receives an HTTP response with 500 status code and an empty exception body</remarks>
public class BaseException : Exception
{
    public BaseException()
    {
    }

    public BaseException(string message) : base(message)
    {
    }
}