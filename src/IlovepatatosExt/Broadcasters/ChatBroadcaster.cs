using Facepunch;
using JetBrains.Annotations;
using Oxide.Core.Plugins;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class ChatBroadcaster : Pool.IPooled
{
    public ulong Steam64;

    private IPlayerProvider _playerProvider;
    private Plugin _plugin;

    private Core.Libraries.Timer.TimerInstance _callback;

#region Getters/Setters

    public bool IsActive { get; private set; }

#endregion

    public static ChatBroadcaster New(IPlayerProvider provider, Plugin plugin = null)
    {
        var broadcaster = PoolUtility.Get<ChatBroadcaster>();
        broadcaster._playerProvider = provider;
        broadcaster._plugin = plugin;

        return broadcaster;
    }

    [Obsolete("Use " + nameof(New) + "() instead to take advantage of the pool system.")]
    public ChatBroadcaster() { }

    [Obsolete("Use " + nameof(New) + "() instead to take advantage of the pool system.")]
    public ChatBroadcaster(IPlayerProvider playerProvider)
    {
        _playerProvider = playerProvider;
    }

    public void Kill()
    {
        IsActive = false;
        TimerUtility.DestroyToPool(ref _callback);
    }

    public void Start(List<ChatMsg> messages, Func<object[]> format = null, Action onComplete = null)
    {
        List<ChatMsg> copy = messages.ToPooledList(); // copy to avoid modifying the original list
        StartOrComplete(copy, format, onComplete);
    }

    private void StartOrComplete(List<ChatMsg> messages, Func<object[]> format = null, Action onComplete = null)
    {
        IsActive = messages.Count > 0;

        if (IsActive)
        {
            ChatMsg msg = messages.GetAtPlusRemove(0);

            TimerUtility.DestroyToPool(ref _callback);
            _callback = TimerUtility.ScheduleOnce(msg.SecondsBefore, () => BroadcastToPlayers(msg, messages, format, onComplete), _plugin);
        }
        else
        {
            PoolUtility.Free(ref messages);
            onComplete?.Invoke();
        }
    }

    private void BroadcastToPlayers(ChatMsg msg, List<ChatMsg> messages, Func<object[]> format = null, Action onComplete = null)
    {
        string text = format == null ? msg.Msg : msg.Msg.FormatNoThrow(format.Invoke());

        IEnumerable<BasePlayer> players = _playerProvider.GetPlayers();
        players.ChatMessage(text, Steam64);

        _callback = TimerUtility.ScheduleOnce(msg.SecondsAfter, () => StartOrComplete(messages, format, onComplete), _plugin);
    }

    void Pool.IPooled.EnterPool()
    {
        IsActive = false;
        _playerProvider = null;
        _plugin = null;
        
        TimerUtility.DestroyToPool(ref _callback);
    }

    void Pool.IPooled.LeavePool() { }
}