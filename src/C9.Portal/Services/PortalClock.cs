using System.Globalization;

namespace C9.Portal.Services;

/// <summary>Formats UTC timestamps and sizes for the UI in the configured display time zone.</summary>
public sealed class PortalClock
{
    public PortalClock(PortalOptions options)
    {
        try { Zone = TimeZoneInfo.FindSystemTimeZoneById(options.DisplayTimeZone); }
        catch (Exception error) when (error is TimeZoneNotFoundException or InvalidTimeZoneException)
        {
            Zone = TimeZoneInfo.CreateCustomTimeZone("UTC+07", TimeSpan.FromHours(7), "UTC+07:00", "UTC+07:00");
        }
    }

    public TimeZoneInfo Zone { get; }
    public string ZoneLabel => "UTC" + (Zone.BaseUtcOffset < TimeSpan.Zero ? "-" : "+") + Zone.BaseUtcOffset.ToString(@"hh\:mm", CultureInfo.InvariantCulture);

    public DateTimeOffset Local(DateTimeOffset value) => TimeZoneInfo.ConvertTime(value, Zone);
    public string Time(DateTimeOffset? value) => value is { } v ? Local(v).ToString("HH:mm:ss", CultureInfo.InvariantCulture) : "—";
    public string Precise(DateTimeOffset? value) => value is { } v ? Local(v).ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture) : "—";
    public string Full(DateTimeOffset? value) => value is { } v ? Local(v).ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) : "—";

    public static string Bytes(long value) => value switch
    {
        < 1024 => value.ToString(CultureInfo.InvariantCulture) + " B",
        < 1024 * 1024 => (value / 1024d).ToString("0.0", CultureInfo.InvariantCulture) + " KiB",
        < 1024L * 1024 * 1024 => (value / 1048576d).ToString("0.0", CultureInfo.InvariantCulture) + " MiB",
        _ => (value / 1073741824d).ToString("0.00", CultureInfo.InvariantCulture) + " GiB"
    };

    public static string Number(long value) => value.ToString("N0", CultureInfo.InvariantCulture);

    public static string Duration(TimeSpan value) => value.TotalSeconds switch
    {
        < 0 => "—",
        < 60 => ((int)value.TotalSeconds).ToString(CultureInfo.InvariantCulture) + " วิ",
        < 3600 => ((int)value.TotalMinutes).ToString(CultureInfo.InvariantCulture) + " นาที",
        < 86400 => value.ToString(@"h\:mm", CultureInfo.InvariantCulture) + " ชม.",
        _ => ((int)value.TotalDays).ToString(CultureInfo.InvariantCulture) + " วัน " + value.Hours.ToString(CultureInfo.InvariantCulture) + " ชม."
    };

    public static string Since(DateTimeOffset? start) => start is { } s ? Duration(DateTimeOffset.UtcNow - s) : "—";

    public static string Type(ushort? type) => type is { } t ? $"0x{t:X4}" : "—";
}
