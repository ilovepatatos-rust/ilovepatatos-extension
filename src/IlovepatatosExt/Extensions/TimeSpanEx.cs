using JetBrains.Annotations;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class TimeSpanEx
{
    extension(TimeSpan from)
    {
        [MustUseReturnValue]
        public TimeSpan TimeUntil(TimeSpan to)
        {
            if (from > to) // 23:00 -> 01:00
                return new TimeSpan(24, 0, 0) - from + to;

            return to - from; // 13:00 -> 17:00
        }

        [MustUseReturnValue]
        public string FormatToTime(string format = "{0:00}:{1:00}:{2:00}")
        {
            float seconds = (float)from.TotalSeconds;
            return seconds.FormatToTime(format);
        }

        [MustUseReturnValue]
        public string FormatToTimeSmart(string hour = "h", string minute = "m", string second = "s")
        {
            float seconds = (float)from.TotalSeconds;
            return seconds.FormatToTimeSmart(hour, minute, second);
        }
    }
}