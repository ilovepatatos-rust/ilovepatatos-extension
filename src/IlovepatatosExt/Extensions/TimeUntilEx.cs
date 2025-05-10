using JetBrains.Annotations;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class TimeUntilEx
{
    [MustUseReturnValue]
    public static int Seconds(this TimeUntil value)
    {
        return ((float)value).Seconds();
    }

    [MustUseReturnValue]
    public static int Minutes(this TimeUntil value)
    {
        return ((float)value).Minutes();
    }

    [MustUseReturnValue]
    public static int Hours(this TimeUntil value)
    {
        return ((float)value).Hours();
    }

    [MustUseReturnValue]
    public static string FormatToTime(this TimeUntil value, string format = "{0:00}:{1:00}:{2:00}")
    {
        return ((float)value).FormatToTime(format);
    }

    [MustUseReturnValue]
    public static string FormatToTimeSmart(this TimeUntil value, string hour = "h", string minute = "m", string second = "s")
    {
        return ((float)value).FormatToTimeSmart(hour, minute, second);
    }
}