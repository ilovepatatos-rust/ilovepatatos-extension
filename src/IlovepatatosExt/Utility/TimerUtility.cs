using JetBrains.Annotations;
using Oxide.Core;
using Oxide.Core.Plugins;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class TimerUtility
{
    public static readonly Oxide.Core.Libraries.Timer TimersPool = Interface.Oxide.GetLibrary<Oxide.Core.Libraries.Timer>("Timer");

    /// <summary>
    /// Returns a callback instance that will be executed after the given delay.
    /// </summary>
    public static Core.Libraries.Timer.TimerInstance ScheduleOnce(float delay, Action action, Plugin owner = null)
    {
        return TimersPool.Once(delay, action, owner);
    }

    /// <summary>
    /// Returns a callback instance that will be executed every interval.
    /// </summary>
    public static Core.Libraries.Timer.TimerInstance ScheduleEvery(float interval, Action action, Plugin owner = null)
    {
        return TimersPool.Repeat(interval, -1, action, owner);
    }

    /// <summary>
    /// Returns a callback instance that will be executed every interval for the given amount of times.
    /// </summary>
    public static Core.Libraries.Timer.TimerInstance ScheduleEvery(float interval, int repetitions, Action action, Plugin owner = null)
    {
        return TimersPool.Repeat(interval, repetitions, action, owner);
    }

    public static void DestroyToPool(ref Plugins.Timer timer)
    {
        timer?.DestroyToPool();
        timer = null;
    }

    public static void DestroyToPool(ref Core.Libraries.Timer.TimerInstance timer)
    {
        timer?.DestroyToPool();
        timer = null;
    }
}