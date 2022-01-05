namespace Lagoo.BusinessLogic.Common.Exceptions.Base;

/// <summary>
///   An Exception which gets thrown by <see cref="Services.HttpService"/>
/// </summary>
public class HttpException : BaseException
{
    public HttpException()
    {
    }

    public HttpException(string message) : base(message)
    {
    }
}