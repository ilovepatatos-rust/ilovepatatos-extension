using JetBrains.Annotations;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class DateTimeUtility
{
    public static DateTime TimezoneToDateTime(string timezone)
    {
        TimeZoneInfo info = TimeZoneInfo.FindSystemTimeZoneById(timezone);
        return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, info);
    }
}