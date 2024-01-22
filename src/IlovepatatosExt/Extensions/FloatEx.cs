using JetBrains.Annotations;
using UnityEngine;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class FloatEx
{
    public static int Seconds(this float value)
    {
        return Mathf.RoundToInt(value).Seconds();
    }

    public static int Minutes(this float value)
    {
        return Mathf.RoundToInt(value).Minutes();
    }

    public static int Hours(this float value)
    {
        return Mathf.RoundToInt(value).Hours();
    }

    public static string FormatToTime(this float value, string format = "{0:00}:{1:00}:{2:00}")
    {
        return Mathf.RoundToInt(value).FormatToTime(format);
    }

    public static string FormatToTimeSmart(this float value, string hour = "h", string minute = "m", string second = "s")
    {
        return Mathf.RoundToInt(value).FormatToTimeSmart(hour, minute, second);
    }
}