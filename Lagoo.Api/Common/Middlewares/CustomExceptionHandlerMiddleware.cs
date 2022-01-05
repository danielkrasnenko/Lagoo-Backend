using System.Net;
using System.Net.Mime;
using System.Text.Json;
using FluentValidation;
using Lagoo.BusinessLogic.Common.Exceptions.Api;

namespace Lagoo.Api.Common.Middlewares;

/// <summary>
///   A middleware for appropriate handling of thrown custom exceptions in the app
/// </summary>
public class CustomExceptionHandlerMiddleware
{
    private readonly ILogger<CustomExceptionHandlerMiddleware> _logger;

    private readonly RequestDelegate _requestDelegate;

    public CustomExceptionHandlerMiddleware(ILogger<CustomExceptionHandlerMiddleware> logger, RequestDelegate requestDelegate)
    {
        _logger = logger;
        _requestDelegate = requestDelegate;
    }

    public Task Invoke(HttpContext httpContext)
    {
        try
        {
            return _requestDelegate(httpContext);
        }
        catch (Exception exception)
        {
            return HandleExceptionAsync(httpContext, exception);
        }
    }

    protected List<string> ToErrorList(IDictionary<string, string[]> errorsMap) =>
        errorsMap.SelectMany(p => p.Value).ToList();

    private Task HandleExceptionAsync(HttpContext httpContext, Exception exception)
    {
        httpContext.Response.ContentType = MediaTypeNames.Application.Json;

        return exception switch
        {
            ValidationException validationException => HandleBadRequestExceptionAsync(new BadRequestException(validationException.Errors), httpContext),
            BadRequestException badRequestException => HandleBadRequestExceptionAsync(badRequestException, httpContext),
            ForbiddenException forbiddenException => HandleForbiddenExceptionAsync(forbiddenException, httpContext),
            NotFoundException notFoundException => HandleNotFoundExceptionAsync(notFoundException, httpContext),
            _ => HandleInternalServerException(exception, httpContext)
        };
    }
    
    private Task HandleBadRequestExceptionAsync(BadRequestException badRequestException, HttpContext httpContext)
    {
        httpContext = SetStatusCode(httpContext, HttpStatusCode.BadRequest);
        
        return badRequestException.ShowErrorCodes
            ? WriteResponseAsync(httpContext, badRequestException.Failures)
            : WriteResponseAsync(httpContext, ToErrorList(badRequestException.Failures));
    }

    private Task HandleForbiddenExceptionAsync(ForbiddenException forbiddenException, HttpContext httpContext)
    {
        httpContext = SetStatusCode(httpContext, HttpStatusCode.Forbidden);

        return WriteResponseAsync(httpContext, ToErrorList(forbiddenException.Errors));
    }

    private Task HandleNotFoundExceptionAsync(NotFoundException notFoundException, HttpContext httpContext)
    {
        httpContext = SetStatusCode(httpContext, HttpStatusCode.NotFound);

        return WriteResponseAsync(httpContext, ToErrorList(notFoundException.Errors));
    }

    private Task HandleInternalServerException(Exception exception, HttpContext httpContext)
    {
        httpContext = SetStatusCode( httpContext, HttpStatusCode.InternalServerError);
        
        _logger.LogError(exception, "{Message}", exception.Message);

        return WriteResponseAsync(httpContext, exception);
    }

    private Task WriteResponseAsync(HttpContext httpContext, object response) =>
        httpContext.Response.WriteAsync(JsonSerializer.Serialize(response));

    private HttpContext SetStatusCode(HttpContext httpContext, HttpStatusCode statusCode)
    {
        httpContext.Response.StatusCode = (int)statusCode;
        return httpContext;
    }
}