using JetBrains.Annotations;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class ChatBroadcaster
{
    public ulong Steam64;
    private readonly IPlayerProvider m_PlayerProvider;
    private Core.Libraries.Timer.TimerInstance m_Callback;

#region Getters/Setters

    public bool IsActive { get; private set; }

#endregion

    public ChatBroadcaster(IPlayerProvider playerProvider)
    {
        m_PlayerProvider = playerProvider;
    }

    public void Kill()
    {
        IsActive = false;
        m_Callback?.Destroy();
    }

    public void Start(List<ChatMsg> messages, Func<object[]> format = null, Action onComplete = null)
    {
        List<ChatMsg> copy = messages.ToList(); // copy to avoid modifying the original list
        InternalStart(copy, format, onComplete);
    }

    private void InternalStart(List<ChatMsg> messages, Func<object[]> format = null, Action onComplete = null)
    {
        IsActive = messages.Count > 0;

        if (IsActive)
        {
            ChatMsg msg = messages.GetAtPlusRemove(0);

            m_Callback?.Destroy();
            m_Callback = TimerUtility.TimersPool.Once(msg.SecondsBefore, () => Continue(msg, messages, format, onComplete));
        }
        else
        {
            onComplete?.Invoke();
        }
    }

    private void Continue(ChatMsg msg, List<ChatMsg> messages, Func<object[]> format = null, Action onComplete = null)
    {
        string text = format == null ? msg.Msg : msg.Msg.FormatNoThrow(format.Invoke());

        var players = m_PlayerProvider.GetPlayers();
        players.ChatMessage(text, Steam64);

        m_Callback = TimerUtility.TimersPool.Once(msg.SecondsAfter, () => InternalStart(messages, format, onComplete));
    }
}