using JetBrains.Annotations;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class DateTimeEx
{
    extension(DateTime time)
    {
        [MustUseReturnValue]
        public TimeSpan ToTimeSpan()
        {
            return new TimeSpan(time.Hour, time.Minute, time.Second);
        }
    }
}