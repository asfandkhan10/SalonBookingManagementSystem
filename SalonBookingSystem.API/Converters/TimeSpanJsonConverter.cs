using System.Text.Json;
using System.Text.Json.Serialization;

namespace SalonBookingSystem.API.Converters;

public class TimeSpanJsonConverter : JsonConverter<TimeSpan>
{
    public override TimeSpan Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        if (string.IsNullOrEmpty(value))
            return TimeSpan.Zero;

        // Try parsing with common formats
        if (TimeSpan.TryParse(value, out var result))
            return result;

        // Try parsing HH:mm:ss format
        if (TimeSpan.TryParseExact(value, @"hh\:mm\:ss", null, out result))
            return result;

        // Try parsing HH:mm format
        if (TimeSpan.TryParseExact(value, @"hh\:mm", null, out result))
            return result;

        throw new JsonException($"Unable to convert '{value}' to TimeSpan.");
    }

    public override void Write(Utf8JsonWriter writer, TimeSpan value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(@"hh\:mm\:ss"));
    }
}
