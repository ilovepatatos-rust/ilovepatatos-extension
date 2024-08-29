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

    private int _currentTime;
    private TimeSince _timeSinceLastUpdate;

    private Timer.TimerInstance _callback;

    private Action _onUpdateCallback, _onCompleteCallback;

#region Getters/Setters

    public bool StopOnCompletion { get; set; } = true;
    public bool IsCounting { get; private set; }
    
    public float Interval { get; private set; }
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

        Interval = interval;
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
        IsCounting = false;
        _currentTime = 0;
        Duration = 0;
        TimerUtility.DestroyToPool(ref _callback);
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
            _currentTime += Mathf.RoundToInt(Interval);

        Update();

        if (StopOnCompletion && timeUntil <= 0)
            OnComplete();
    }

    protected virtual void OnComplete()
    {
        IsCounting = false;
        TimerUtility.DestroyToPool(ref _callback);
        
        _onCompleteCallback?.Invoke();
    }

    void Pool.IPooled.EnterPool()
    {
        PluginOwner = null;
        _currentTime = 0;
        _timeSinceLastUpdate = 0;

        _callback = null;
        _onUpdateCallback = null;
        _onCompleteCallback = null;
        
        StopOnCompletion = true;
        IsCounting = false;
        
        Interval = 0f;
        Duration = 0;
        
        TimerUtility.DestroyToPool(ref _callback);
    }

    void Pool.IPooled.LeavePool() { }
}