using JetBrains.Annotations;
using Oxide.Ext.ConsoleExt;
using Oxide.Plugins;
using UnityEngine;
using Timer = Oxide.Plugins.Timer;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class WeeklyCycles
{
    public CycleSettings CycleSettings;

    private Timer _callback;
    private readonly PluginTimers _pluginTimers;
    private readonly IConsoleLogger _console;

    public event Action OnEnable, OnDisable;

#region Getters/Setters

    public string Name { get; }

    public bool IsEnabled { get; private set; }

    public bool IsAlwaysActive { get; private set; }

    public bool WillSomedayBeEnable => CycleSettings.ActiveDays.Count > 0;

    public TimeSpan TimeUntilEnabled
    {
        get
        {
            if (IsEnabled)
                return new TimeSpan(0, 0, 0);

            if (!WillSomedayBeEnable)
                return new TimeSpan(0, 0, 0);

            DateTime now = GetCurrentDatetimeAtTimezone();
            DailySettings settings = GetNextActivationDayFrom(now);

            int amountDaysUntilActiveDay = GetAmountDaysUntil(settings);
            TimeSpan objective = new TimeSpan(amountDaysUntilActiveDay * 24, 0, 0) + settings.ActivationTime;

            var time = now.ToTimeSpan();
            return time.TimeUntil(objective);
        }
    }

    public TimeSpan TimeUntilDisabled
    {
        get
        {
            if (!IsEnabled)
                return new TimeSpan(0, 0, 0);

            DateTime now = GetCurrentDatetimeAtTimezone();
            DailySettings settings = GetNextActivationDayFrom(now);

            var time = now.ToTimeSpan();
            TimeSpan duration = GetTotalDuration(time, settings);
            TimeSpan objective = time + duration;

            return time.TimeUntil(objective);
        }
    }

#endregion

    public WeeklyCycles(string name, PluginTimers timers, CycleSettings settings, IConsoleLogger console = null)
    {
        Name = name;
        _pluginTimers = timers ?? throw new ArgumentNullException(nameof(timers));
        CycleSettings = settings ?? throw new ArgumentNullException(nameof(settings));
        _console = console;

        IsAlwaysActive = settings.IsAlwaysEnable;
    }

    public void Initialize()
    {
        Unload();

        if (IsAlwaysActive)
        {
            IsEnabled = true;
            OnEnable?.Invoke();
        }
        else if (CycleSettings.ActiveDays.Count > 0)
        {
            ScheduleEnabling();
        }
    }

    public void Unload()
    {
        if (IsEnabled)
        {
            IsEnabled = false;
            OnDisable?.Invoke();
        }

        TimerUtility.DestroyToPool(ref _callback);
    }

    private void Enable()
    {
        IsEnabled = true;
        _console?.WriteLine($"Enabling cycle {Name} at {DateTime.Now}");

        OnEnable?.Invoke();
        ScheduleDisabling();
    }

    private void Disable()
    {
        IsEnabled = false;
        _console?.WriteLine($"Disabling cycle {Name} at {DateTime.Now}");

        OnDisable?.Invoke();
        ScheduleEnabling();
    }

    private void ScheduleEnabling()
    {
        DateTime now = GetCurrentDatetimeAtTimezone();
        DailySettings settings = GetNextActivationDayFrom(now);

        bool isActiveDay = IsActiveDay(settings);
        bool isActiveHours = isActiveDay && settings.IsBetweenActiveHours(now);

        if (isActiveHours)
        {
            Enable();
        }
        else
        {
            int amountDaysUntilActiveDay = GetAmountDaysUntil(settings);
            TimeSpan objective = new TimeSpan(amountDaysUntilActiveDay * 24, 0, 0) + settings.ActivationTime;

            var time = now.ToTimeSpan();
            TimeSpan timeUntil = time.TimeUntil(objective);

            float seconds = Mathf.RoundToInt((float)timeUntil.TotalSeconds) + 1;
            _console?.WriteLine($"Scheduling enabling of {Name} in {timeUntil} at {now + timeUntil} (Now: {now})");

            _callback?.Destroy();
            _callback = _pluginTimers.Once(seconds, Enable);
        }
    }

    private void ScheduleDisabling()
    {
        DateTime now = GetCurrentDatetimeAtTimezone();
        DailySettings settings = GetNextActivationDayFrom(now);

        bool isActiveDay = IsActiveDay(settings);
        bool isActiveHours = isActiveDay && settings.IsBetweenActiveHours(now);

        if (isActiveHours)
        {
            var time = now.ToTimeSpan();
            TimeSpan duration = GetTotalDuration(time, settings);

            float seconds = (float)duration.TotalSeconds;
            _console?.WriteLine($"Scheduling disabling of {Name} in {duration} at {now + duration} (Now: {now})");

            _callback?.Destroy();
            _callback = _pluginTimers.Once(seconds, Disable);
        }
        else
        {
            Disable();
        }
    }

    private DateTime GetCurrentDatetimeAtTimezone()
    {
        return DateTimeUtility.TimezoneToDateTime(CycleSettings.Timezone);
    }

    private DailySettings GetNextActivationDayFrom(DateTime now)
    {
        var time = now.ToTimeSpan();
        return CycleSettings.GetNextActivationDayFrom(now.DayOfWeek, time);
    }

    private bool IsActiveDay(DailySettings settings)
    {
        DateTime now = GetCurrentDatetimeAtTimezone();
        return string.Equals($"{now.DayOfWeek}", settings.Day);
    }

    private int GetAmountDaysUntil(DailySettings settings)
    {
        DateTime now = GetCurrentDatetimeAtTimezone();
        return settings.AmountDaysUntilFrom(now);
    }

    // Recursive
    private TimeSpan GetTotalDuration(TimeSpan from, DailySettings settings)
    {
        if (!Enum.TryParse(settings.Day, out DayOfWeek day))
            throw new InvalidCastException($"{nameof(DailySettings.Day)} isn't parsable! Value: {settings.Day}");

        DailySettings next = CycleSettings.GetNextActivationDayFrom(day, settings.DeactivationTime);
        if (next == null) return new TimeSpan(0, 0, 0);

        TimeSpan timeUntil = from.TimeUntil(settings.DeactivationTime);

        // Is more than one second apart ?
        if (settings.DeactivationTime.TimeUntil(next.ActivationTime).TotalSeconds > 1)
            return timeUntil;

        // Is diff day and more than one day apart ?
        if (!string.Equals(settings.Day, next.Day) && !string.Equals(day.NextDay().ToString(), next.Day))
            return timeUntil;

        return timeUntil + GetTotalDuration(next.ActivationTime, next);
    }
}