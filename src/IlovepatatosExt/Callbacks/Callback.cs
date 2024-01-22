using JetBrains.Annotations;
using UnityEngine;
using Timer = Oxide.Core.Libraries.Timer;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class Callback
{
    private float m_Interval;
    private int m_CurrentTime;
    private TimeSince m_TimeSinceLastUpdate;

    private Timer.TimerInstance m_Callback;

    private Action m_OnUpdateCallback, m_OnCompleteCallback;

#region Getters/Setters

    public bool StopOnCompletion { get; set; } = true;
    public bool IsCounting { get; private set; }
    public int Duration { get; private set; }

    public int TimeUntil => Math.Max(0, Duration - m_CurrentTime);
    public TimeSince TimeSinceStart { get; private set; }

    public int Hours => TimeUntil / IntegerEx.HOUR;
    public int Minutes => TimeUntil % IntegerEx.HOUR / IntegerEx.MINUTE;
    public int Seconds => TimeUntil % IntegerEx.HOUR % IntegerEx.MINUTE;

#endregion

    public virtual void Start(int duration, float interval = 1f, Action onUpdate = null, Action onComplete = null)
    {
        IsCounting = true;
        m_CurrentTime = 0;

        Duration = duration;
        TimeSinceStart = 0;

        m_Interval = interval;
        m_TimeSinceLastUpdate = interval;

        m_OnUpdateCallback = onUpdate;
        m_OnCompleteCallback = onComplete;

        m_Callback?.Destroy();

        if (!StopOnCompletion || Duration > 0)
            m_Callback = TimerUtility.TimersPool.Repeat(interval, -1, InternalUpdate);

        InternalUpdate();
    }

    public virtual void Cancel()
    {
        Duration = 0;
        m_CurrentTime = 0;
        IsCounting = false;
        m_Callback?.Destroy();
    }

    public void Add(int seconds)
    {
        Duration += seconds;
    }

    protected virtual void Update()
    {
        m_OnUpdateCallback?.Invoke();
    }

    private void InternalUpdate()
    {
        int timeUntil = TimeUntil;

        if (timeUntil > 0)
            m_CurrentTime += Mathf.RoundToInt(m_Interval);

        Update();

        if (StopOnCompletion && timeUntil <= 0)
            OnComplete();
    }

    protected virtual void OnComplete()
    {
        IsCounting = false;
        m_Callback?.Destroy();
        m_OnCompleteCallback?.Invoke();
    }
}