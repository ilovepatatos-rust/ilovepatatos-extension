using JetBrains.Annotations;
using UnityEngine;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class FloatEx
{
    [MustUseReturnValue]
    public static int ToInt(this float value)
    {
        return Mathf.RoundToInt(value);
    }

    [MustUseReturnValue]
    public static int FloorToInt(this float value)
    {
        return Mathf.Floor(value).ToInt();
    }

    [MustUseReturnValue]
    public static int CeilToInt(this float value)
    {
        return Mathf.Ceil(value).ToInt();
    }

    [MustUseReturnValue]
    public static int Seconds(this float value)
    {
        return Mathf.RoundToInt(value).Seconds();
    }

    [MustUseReturnValue]
    public static int Minutes(this float value)
    {
        return Mathf.RoundToInt(value).Minutes();
    }

    [MustUseReturnValue]
    public static int Hours(this float value)
    {
        return Mathf.RoundToInt(value).Hours();
    }

    [MustUseReturnValue]
    public static string FormatToTime(this float value, string format = "{0:00}:{1:00}:{2:00}")
    {
        return Mathf.RoundToInt(value).FormatToTime(format);
    }

    [MustUseReturnValue]
    public static string FormatToTimeSmart(this float value, string hour = "h", string minute = "m", string second = "s")
    {
        return Mathf.RoundToInt(value).FormatToTimeSmart(hour, minute, second);
    }
}