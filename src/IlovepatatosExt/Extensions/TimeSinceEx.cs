using JetBrains.Annotations;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class TimeSinceEx
{
    public static int Seconds(this TimeSince value)
    {
        return ((float)value).Seconds();
    }

    public static int Minutes(this TimeSince value)
    {
        return ((float)value).Minutes();
    }

    public static int Hours(this TimeSince value)
    {
        return ((float)value).Hours();
    }

    public static string FormatToTime(this TimeSince value, string format = "{0:00}:{1:00}:{2:00}")
    {
        return ((float)value).FormatToTime(format);
    }

    public static string FormatToTimeSmart(this TimeSince value, string hour = "h", string minute = "m", string second = "s")
    {
        return ((float)value).FormatToTimeSmart(hour, minute, second);
    }
}