namespace Lagoo.BusinessLogic.Common.Exceptions.Base;

/// <summary>
///   An Exception gets thrown if an application is inappropriate
/// </summary>
public class InvalidAppStateException : BaseException
{
    public InvalidAppStateException()
    {
    }

    public InvalidAppStateException(string message) : base(message)
    {
    }
}