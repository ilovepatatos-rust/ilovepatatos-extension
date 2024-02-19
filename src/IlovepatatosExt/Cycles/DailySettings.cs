using Newtonsoft.Json;

namespace Oxide.Ext.IlovepatatosExt;

[Serializable]
public class DailySettings
{
    [JsonProperty("Day")]
    public string Day = "Monday";

    [JsonProperty("Activation time - [00:00:00 -> 23:59:59]")]
    public TimeSpan ActivationTime;

    [JsonProperty("Deactivation time - [00:00:00 -> 23:59:59]")]
    public TimeSpan DeactivationTime;

    public bool IsBetweenActiveHours(DateTime now)
    {
        if (!string.Equals($"{now.DayOfWeek}", Day))
            return false;

        var time = now.ToTimeSpan();
        return ActivationTime <= time && time < DeactivationTime; // 05:00am - 13:00pm
    }

    public int AmountDaysUntilFrom(DateTime now)
    {
        var time = now.ToTimeSpan();
        DayOfWeek day = now.DayOfWeek;

        int amount = 0;
        int initial = (int)day;

        for (int i = initial; i < initial + DayOfWeekEx.AmountDays; i++)
        {
            int value = i % DayOfWeekEx.AmountDays;
            string name = $"{(DayOfWeek)value}";

            if (!string.Equals(Day, name) || i == initial && time > DeactivationTime)
            {
                amount++;
            }
            else
            {
                break;
            }
        }

        return amount;
    }
}