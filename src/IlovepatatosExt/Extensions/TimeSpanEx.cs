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
}