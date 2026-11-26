using JetBrains.Annotations;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class IntegerEx
{
    public const int HOUR = 60 * MINUTE;
    public const int MINUTE = 60 * SECOND;
    public const int SECOND = 1;

    extension(int value)
    {
        [MustUseReturnValue]
        public int GetAmountDigits()
        {
            double log = Math.Log10(value);
            double floor = Math.Floor(log + 1);
            return (int)floor;
        }

        /// <summary>
        /// Returns only the seconds, excluding minutes and hours.
        /// </summary>
        [MustUseReturnValue]
        public int Seconds()
        {
            return value % HOUR % MINUTE;
        }

        /// <summary>
        /// Returns the total amount of seconds.
        /// </summary>
        [MustUseReturnValue]
        public int TotalSeconds()
        {
            return value;
        }

        /// <summary>
        /// Returns only the minutes, excluding hours.
        /// </summary>
        [MustUseReturnValue]
        public int Minutes()
        {
            return value % HOUR / MINUTE;
        }

        /// <summary>
        /// Returns the total amount of minutes.
        /// </summary>
        [MustUseReturnValue]
        public int TotalMinutes()
        {
            return value / MINUTE;
        }

        /// <summary>
        /// Returns only the hours.
        /// </summary>
        [MustUseReturnValue]
        public int Hours()
        {
            return value / HOUR;
        }

        /// <summary>
        /// Returns the total amount of hours.
        /// </summary>
        [MustUseReturnValue]
        public int TotalHours()
        {
            return value / HOUR;
        }

        [MustUseReturnValue]
        public string FormatToTime(string format = "{0:00}:{1:00}:{2:00}")
        {
            int hours = value.Hours();
            int minutes = value.Minutes();
            int seconds = value.Seconds();

            return string.Format(format, hours, minutes, seconds);
        }

        [MustUseReturnValue]
        public string FormatToTimeSmart(string hour = "h", string minute = "m", string second = "s")
        {
            int hours = value.Hours();
            int minutes = value.Minutes();

            if (hours > 0)
            {
                string format = minutes == 0 ? $"{{0}}{hour}" : $"{{0}}{hour} {{1:00}}{minute}";
                return value.FormatToTime(format);
            }

            if (minutes > 0)
                return value.FormatToTime($"{{1}}{minute}");

            return value.FormatToTime($"{{2}}{second}");
        }

        [MustUseReturnValue]
        public bool ContainsTopology(TerrainTopology.Enum topology)
        {
            return topology == (TerrainTopology.Enum)~0 || (value & (int)topology) == (int)topology;
        }
    }
}