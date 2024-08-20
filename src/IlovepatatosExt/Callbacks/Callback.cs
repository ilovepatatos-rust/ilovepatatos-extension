using Facepunch;
using JetBrains.Annotations;
using Oxide.Core.Plugins;
using UnityEngine;
using Timer = Oxide.Core.Libraries.Timer;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class Callback : Pool.IPooled
{
    public Plugin PluginOwner;

    private float _interval;
    private int _currentTime;
    private TimeSince _timeSinceLastUpdate;

    private Timer.TimerInstance _callback;

    private Action _onUpdateCallback, _onCompleteCallback;

#region Getters/Setters

    public bool StopOnCompletion { get; set; } = true;
    public bool IsCounting { get; private set; }
    public int Duration { get; private set; }

    public int TimeUntil => Math.Max(0, Duration - _currentTime);
    public TimeSince TimeSinceStart { get; private set; }

    public int Hours => TimeUntil / IntegerEx.HOUR;
    public int Minutes => TimeUntil % IntegerEx.HOUR / IntegerEx.MINUTE;
    public int Seconds => TimeUntil % IntegerEx.HOUR % IntegerEx.MINUTE;

#endregion

    public virtual void Start(int duration, float interval = 1f, Action onUpdate = null, Action onComplete = null)
    {
        IsCounting = true;
        _currentTime = 0;

        Duration = duration;
        TimeSinceStart = 0;

        _interval = interval;
        _timeSinceLastUpdate = interval;

        _onUpdateCallback = onUpdate;
        _onCompleteCallback = onComplete;

        _callback?.DestroyToPool();

        if (!StopOnCompletion || Duration > 0)
            _callback = TimerUtility.TimersPool.Repeat(interval, -1, InternalUpdate, PluginOwner);

        InternalUpdate();
    }

    public virtual void Cancel()
    {
        Duration = 0;
        _currentTime = 0;
        IsCounting = false;
        _callback?.DestroyToPool();
    }

    public void Add(int seconds)
    {
        Duration += seconds;
    }

    protected virtual void Update()
    {
        _onUpdateCallback?.Invoke();
    }

    private void InternalUpdate()
    {
        int timeUntil = TimeUntil;

        if (timeUntil > 0)
            _currentTime += Mathf.RoundToInt(_interval);

        Update();

        if (StopOnCompletion && timeUntil <= 0)
            OnComplete();
    }

    protected virtual void OnComplete()
    {
        IsCounting = false;
        _callback?.Destroy();
        _onCompleteCallback?.Invoke();
    }

    void Pool.IPooled.EnterPool()
    {
        Cancel();

        PluginOwner = null;
        _interval = 0f;
        
        _callback = null;
        _onUpdateCallback = null;
        _onCompleteCallback = null;
    }

    void Pool.IPooled.LeavePool() { }
}