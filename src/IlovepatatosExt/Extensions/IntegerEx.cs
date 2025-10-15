using JetBrains.Annotations;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class IntegerEx
{
    public const int HOUR = 60 * MINUTE;
    public const int MINUTE = 60 * SECOND;
    public const int SECOND = 1;

    [MustUseReturnValue]
    public static int GetAmountDigits(this int value)
    {
        double log = Math.Log10(value);
        double floor = Math.Floor(log + 1);
        return (int)floor;
    }

    /// <summary>
    /// Returns only the seconds, excluding minutes and hours.
    /// </summary>
    [MustUseReturnValue]
    public static int Seconds(this int value)
    {
        return value % HOUR % MINUTE;
    }

    /// <summary>
    /// Returns the total amount of seconds.
    /// </summary>
    [MustUseReturnValue]
    public static int TotalSeconds(this int value)
    {
        return value;
    }

    /// <summary>
    /// Returns only the minutes, excluding hours.
    /// </summary>
    [MustUseReturnValue]
    public static int Minutes(this int value)
    {
        return value % HOUR / MINUTE;
    }

    /// <summary>
    /// Returns the total amount of minutes.
    /// </summary>
    [MustUseReturnValue]
    public static int TotalMinutes(this int value)
    {
        return value / MINUTE;
    }

    /// <summary>
    /// Returns only the hours.
    /// </summary>
    [MustUseReturnValue]
    public static int Hours(this int value)
    {
        return value / HOUR;
    }

    /// <summary>
    /// Returns the total amount of hours.
    /// </summary>
    [MustUseReturnValue]
    public static int TotalHours(this int value)
    {
        return value / HOUR;
    }

    [MustUseReturnValue]
    public static string FormatToTime(this int value, string format = "{0:00}:{1:00}:{2:00}")
    {
        int hours = value.Hours();
        int minutes = value.Minutes();
        int seconds = value.Seconds();

        return string.Format(format, hours, minutes, seconds);
    }

    [MustUseReturnValue]
    public static string FormatToTimeSmart(this int value, string hour = "h", string minute = "m", string second = "s")
    {
        int hours = value.Hours();
        int minutes = value.Minutes();

        if (hours > 0)
        {
            string format = minutes == 0 ? $"{{0}}{hour}" : $"{{0}}{hour} {{1:00}}{minute}";
            return value.FormatToTime(format);
        }

        if (minutes > 0)
            return value.FormatToTime($"{{1}}{minute}");

        return value.FormatToTime($"{{2}}{second}");
    }

    [MustUseReturnValue]
    public static bool ContainsTopology(this int value, TerrainTopology.Enum topology)
    {
        return topology == (TerrainTopology.Enum)~0 || (value & (int)topology) == (int)topology;
    }
}