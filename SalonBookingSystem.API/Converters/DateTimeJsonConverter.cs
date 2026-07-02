using System.Text.Json;
using System.Text.Json.Serialization;

namespace SalonBookingSystem.API.Converters;

public class DateTimeJsonConverter : JsonConverter<DateTime>
{
    private const string DateOnlyFormat = "yyyy-MM-dd";
    private const string DateTimeFormat = "yyyy-MM-ddTHH:mm:ss";

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        if (string.IsNullOrEmpty(value))
            return DateTime.MinValue;

        if (DateTime.TryParse(value, null, System.Globalization.DateTimeStyles.RoundtripKind, out var result))
            return result;

        if (DateTime.TryParseExact(value, DateOnlyFormat, null, System.Globalization.DateTimeStyles.None, out result))
            return result;

        throw new JsonException($"Unable to convert '{value}' to DateTime.");
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        // Date-only values (time component is exactly midnight) are written as yyyy-MM-dd.
        // Audit timestamps and any value with a time component are written as full ISO 8601.
        if (value.TimeOfDay == TimeSpan.Zero)
            writer.WriteStringValue(value.ToString(DateOnlyFormat));
        else
            writer.WriteStringValue(value.ToString(DateTimeFormat));
    }
}
