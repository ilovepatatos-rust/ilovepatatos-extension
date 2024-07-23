using JetBrains.Annotations;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class TimeSpanEx
{
    public static TimeSpan TimeUntil(this TimeSpan from, TimeSpan to)
    {
        if (from > to) // 23:00 -> 01:00
            return new TimeSpan(24, 0, 0) - from + to;

        return to - from; // 13:00 -> 17:00
    }

    public static string FormatToTime(this TimeSpan value, string format = "{0:00}:{1:00}:{2:00}")
    {
        float seconds = (float)value.TotalSeconds;
        return seconds.FormatToTime(format);
    }

    public static string FormatToTimeSmart(this TimeSpan value, string hour = "h", string minute = "m", string second = "s")
    {
        float seconds = (float)value.TotalSeconds;
        return seconds.FormatToTimeSmart(hour, minute, second);
    }
}