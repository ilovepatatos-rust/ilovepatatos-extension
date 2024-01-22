using JetBrains.Annotations;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class IntegerEx
{
    public const int HOUR = 60 * MINUTE;
    public const int MINUTE = 60 * SECOND;
    public const int SECOND = 1;

    public static int Seconds(this int value)
    {
        return value % HOUR % MINUTE;
    }

    public static int Minutes(this int value)
    {
        return value % HOUR / MINUTE;
    }

    public static int Hours(this int value)
    {
        return value / HOUR;
    }

    public static string FormatToTime(this int value, string format = "{0:00}:{1:00}:{2:00}")
    {
        int hours = value.Hours();
        int minutes = value.Minutes();
        int seconds = value.Seconds();

        return string.Format(format, hours, minutes, seconds);
    }

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

    public static bool ContainsTopology(this int value, TerrainTopology.Enum topology)
    {
        return (value & (int)topology) == (int)topology;
    }
}