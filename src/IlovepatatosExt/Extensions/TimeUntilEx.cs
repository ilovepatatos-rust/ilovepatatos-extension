using JetBrains.Annotations;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class TimeUntilEx
{
    extension(TimeUntil value)
    {
        [MustUseReturnValue]
        public int Seconds()
        {
            return ((float)value).Seconds();
        }

        [MustUseReturnValue]
        public int TotalSeconds()
        {
            return ((float)value).TotalSeconds();
        }

        [MustUseReturnValue]
        public int Minutes()
        {
            return ((float)value).Minutes();
        }

        [MustUseReturnValue]
        public int TotalMinutes()
        {
            return ((float)value).TotalMinutes();
        }

        [MustUseReturnValue]
        public int Hours()
        {
            return ((float)value).Hours();
        }

        [MustUseReturnValue]
        public int TotalHours()
        {
            return ((float)value).TotalHours();
        }

        [MustUseReturnValue]
        public string FormatToTime(string format = "{0:00}:{1:00}:{2:00}")
        {
            return ((float)value).FormatToTime(format);
        }

        [MustUseReturnValue]
        public string FormatToTimeSmart(string hour = "h", string minute = "m", string second = "s")
        {
            return ((float)value).FormatToTimeSmart(hour, minute, second);
        }
    }
}