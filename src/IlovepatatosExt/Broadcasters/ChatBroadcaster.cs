using Facepunch;
using JetBrains.Annotations;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class ChatBroadcaster : Pool.IPooled
{
    public ulong Steam64;

    private IPlayerProvider _playerProvider;
    private Core.Libraries.Timer.TimerInstance _callback;

#region Getters/Setters

    public bool IsActive { get; private set; }

#endregion

    public static ChatBroadcaster New(IPlayerProvider provider)
    {
        var broadcaster = PoolUtility.Get<ChatBroadcaster>();
        broadcaster._playerProvider = provider;

        return broadcaster;
    }

    public ChatBroadcaster() { }

    public ChatBroadcaster(IPlayerProvider playerProvider)
    {
        _playerProvider = playerProvider;
    }

    public void Kill()
    {
        IsActive = false;
        _callback?.Destroy();
    }

    public void Start(List<ChatMsg> messages, Func<object[]> format = null, Action onComplete = null)
    {
        List<ChatMsg> copy = messages.ToPooledList(); // copy to avoid modifying the original list
        InternalStart(copy, format, onComplete);
    }

    private void InternalStart(List<ChatMsg> messages, Func<object[]> format = null, Action onComplete = null)
    {
        IsActive = messages.Count > 0;

        if (IsActive)
        {
            ChatMsg msg = messages.GetAtPlusRemove(0);

            _callback?.Destroy();
            _callback = TimerUtility.TimersPool.Once(msg.SecondsBefore, () => Continue(msg, messages, format, onComplete));
        }
        else
        {
            PoolUtility.Free(ref messages);
            onComplete?.Invoke();
        }
    }

    private void Continue(ChatMsg msg, List<ChatMsg> messages, Func<object[]> format = null, Action onComplete = null)
    {
        string text = format == null ? msg.Msg : msg.Msg.FormatNoThrow(format.Invoke());

        IEnumerable<BasePlayer> players = _playerProvider.GetPlayers();
        players.ChatMessage(text, Steam64);

        _callback = TimerUtility.TimersPool.Once(msg.SecondsAfter, () => InternalStart(messages, format, onComplete));
    }

    void Pool.IPooled.EnterPool()
    {
        IsActive = false;
        _playerProvider = null;
        _callback?.Destroy();
    }

    void Pool.IPooled.LeavePool() { }
}