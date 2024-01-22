using JetBrains.Annotations;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class DateTimeEx
{
    public static TimeSpan ToTimeSpan(this DateTime time)
    {
        return new TimeSpan(time.Hour, time.Minute, time.Second);
    }
}