namespace Lagoo.Domain.Extensions;

public static class DateTimeExtensions
{
    public static DateTime ConvertToUtc(this DateTime source) => source.Kind switch
    {
        DateTimeKind.Unspecified => DateTime.SpecifyKind(source, DateTimeKind.Utc),
        DateTimeKind.Local => source.ToUniversalTime(),
        DateTimeKind.Utc => source,
        _ => throw new ArgumentException()
    };

    public static DateTime ConvertToLocal(this DateTime source) => source.Kind switch
    {
        DateTimeKind.Unspecified => DateTime.SpecifyKind(source, DateTimeKind.Local),
        DateTimeKind.Utc => source.ToLocalTime(),
        DateTimeKind.Local => source,
        _ => throw new ArgumentException()
    };
}