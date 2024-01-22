using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Oxide.Ext.IlovepatatosExt;

[Serializable]
public class CycleSettings
{
    [JsonProperty("Is always enable")]
    public bool IsAlwaysEnable;

    [JsonProperty("Timezone")]
    public string Timezone { get; set; } = TimeZoneInfo.Local.Id; // Default to local timezone

    [JsonProperty("Active days - [Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday]", ObjectCreationHandling = ObjectCreationHandling.Replace)]
    public List<DailySettings> ActiveDays = new()
    {
        new DailySettings { ActivationTime = new TimeSpan(22, 0, 0), DeactivationTime = new TimeSpan(23, 59, 59) }
    };

    public DailySettings GetNextActivationDayFrom(DayOfWeek day, TimeSpan from)
    {
        int initial = (int)day;

        for (int i = initial; i <= initial + DayOfWeekEx.AmountDays; i++)
        {
            int value = i % DayOfWeekEx.AmountDays;
            string name = $"{(DayOfWeek)value}";

            foreach (var settings in ActiveDays)
            {
                if (!string.Equals(settings.Day, name))
                    continue;

                if (i == initial && from > settings.ActivationTime && from >= settings.DeactivationTime)
                    continue;

                return settings;
            }
        }

        return null;
    }

    [UsedImplicitly]
    public static CycleSettings Schedule(string timezone, TimeSpan delay, TimeSpan duration)
    {
        CycleSettings cycle = new();
        cycle.ActiveDays.Clear();

        DateTime now = DateTimeUtility.TimezoneToDateTime(timezone);
        TimeSpan time = now.ToTimeSpan();

        DailySettings settings = new();
        settings.Day = $"{now.DayOfWeek}";
        settings.ActivationTime = time + delay;
        settings.DeactivationTime = time + delay + duration;

        cycle.ActiveDays.Add(settings);
        return cycle;
    }
}