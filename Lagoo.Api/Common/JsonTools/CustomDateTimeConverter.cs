using System.Globalization;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Lagoo.Domain.Extensions;

namespace Lagoo.Api.Common.JsonTools;

/// <summary>
///   Converter of <see cref="DateTime"/> for keeping all of them in UTC
///   in communication between a server and client
/// </summary>
public class CustomDateTimeConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (DateTime.TryParse(reader.GetString(), new DateTimeFormatInfo(), DateTimeStyles.AdjustToUniversal, out var parsedDateTime))
        {
            return parsedDateTime;
        }

        throw new SerializationException("Invalid datetime format");
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ConvertToUtc());
    }
}