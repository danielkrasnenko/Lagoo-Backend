using Lagoo.Api.Common.Middlewares;

namespace Lagoo.Api.Common.Extensions;

/// <summary>
///   Custom exception handler extenstion method for <see cref="WebApplication"/>
/// </summary>
public static class CustomExceptionHandlerAppBuilderExtensions
{
    public static void UseCustomExceptionHandler(this IApplicationBuilder applicationBuilder)
    {
        applicationBuilder.UseMiddleware<CustomExceptionHandlerMiddleware>();
    }
}