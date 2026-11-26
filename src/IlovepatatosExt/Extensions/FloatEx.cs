using JetBrains.Annotations;
using UnityEngine;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class FloatEx
{
    extension(float value)
    {
        [MustUseReturnValue]
        public int ToInt()
        {
            return Mathf.RoundToInt(value);
        }

        [MustUseReturnValue]
        public int FloorToInt()
        {
            return Mathf.Floor(value)
                .ToInt();
        }

        [MustUseReturnValue]
        public int CeilToInt()
        {
            return Mathf.Ceil(value)
                .ToInt();
        }

        [MustUseReturnValue]
        public int Seconds()
        {
            return Mathf.RoundToInt(value)
                .Seconds();
        }

        [MustUseReturnValue]
        public int TotalSeconds()
        {
            return Mathf.RoundToInt(value)
                .TotalSeconds();
        }

        [MustUseReturnValue]
        public int Minutes()
        {
            return Mathf.RoundToInt(value)
                .Minutes();
        }

        [MustUseReturnValue]
        public int TotalMinutes()
        {
            return Mathf.RoundToInt(value)
                .TotalMinutes();
        }

        [MustUseReturnValue]
        public int Hours()
        {
            return Mathf.RoundToInt(value)
                .Hours();
        }

        [MustUseReturnValue]
        public int TotalHours()
        {
            return Mathf.RoundToInt(value)
                .TotalHours();
        }

        [MustUseReturnValue]
        public string FormatToTime(string format = "{0:00}:{1:00}:{2:00}")
        {
            return Mathf.RoundToInt(value).FormatToTime(format);
        }

        [MustUseReturnValue]
        public string FormatToTimeSmart(string hour = "h", string minute = "m", string second = "s")
        {
            return Mathf.RoundToInt(value).FormatToTimeSmart(hour, minute, second);
        }
    }
}