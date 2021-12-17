using Lagoo.Api.Common.Middlewares;

namespace Lagoo.Api.Common.Extensions;

public static class CustomAppBuilderExtensions
{
    public static void UseCustomExceptionHandler(this IApplicationBuilder applicationBuilder)
    {
        applicationBuilder.UseMiddleware<CustomExceptionHandlerMiddleware>();
    }
}