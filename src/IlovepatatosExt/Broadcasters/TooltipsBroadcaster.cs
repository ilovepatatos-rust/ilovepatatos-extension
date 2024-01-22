using JetBrains.Annotations;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class TooltipsBroadcaster
{
    private readonly IPlayerProvider m_PlayerProvider;
    private Core.Libraries.Timer.TimerInstance m_Callback;

#region Getters/Setters

    public bool IsActive { get; private set; }

#endregion

    public TooltipsBroadcaster(IPlayerProvider playerProvider)
    {
        m_PlayerProvider = playerProvider;
    }

    public void Kill()
    {
        IsActive = false;
        m_Callback?.Destroy();
    }

    public void Start(List<TooltipMsg> messages, Func<object[]> format = null, Action onComplete = null)
    {
        List<TooltipMsg> copy = messages.ToList(); // copy to avoid modifying the original list
        InternalStart(copy, format, onComplete);
    }

    public void InternalStart(List<TooltipMsg> messages, Func<object[]> format = null, Action onComplete = null)
    {
        IsActive = messages.Count > 0;

        if (IsActive)
        {
            TooltipMsg msg = messages.GetAtPlusRemove(0);

            m_Callback?.Destroy();
            m_Callback = TimerUtility.TimersPool.Once(msg.SecondsBefore, () => Continue(msg, messages, format, onComplete));
        }
        else
        {
            onComplete?.Invoke();
        }
    }

    private void Continue(TooltipMsg msg, List<TooltipMsg> messages, Func<object[]> format = null, Action onComplete = null)
    {
        GameTip.Styles style = msg.Style;
        string text = format == null ? msg.Msg : string.Format(msg.Msg, format.Invoke());

        var players = m_PlayerProvider.GetPlayers();
        players.ShowToast(style, text);

        m_Callback = TimerUtility.TimersPool.Once(msg.SecondsAfter, () => InternalStart(messages, format, onComplete));
    }
}