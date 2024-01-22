using JetBrains.Annotations;
using Oxide.Ext.UiFramework.Builder.Cached;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public abstract class ClockUserInterface : Clock
{
    protected CachedUiBuilder UserInterface { get; set; }
    protected CachedUiBuilder TimeUserInterface { get; set; }

    protected readonly Dictionary<ulong, BasePlayer> Players = new();

    public override void Start(int duration, float interval = 1, Action onUpdate = null, Action onComplete = null)
    {
        UserInterface = CreateUserInterface();
        base.Start(duration, interval, onUpdate, onComplete);
    }

    public virtual void Activate(BasePlayer player)
    {
        if (player == null)
            throw new NullReferenceException($"{nameof(player)} cannot be null!");

        Players.TryAdd(player.userID, player);
        ActivateUserInterface(player);
    }

    public virtual void Activate(IEnumerable<BasePlayer> players)
    {
        foreach (BasePlayer player in players)
            Players.TryAdd(player.userID, player);

        ActivateUserInterface(players);
    }

    public virtual void Deactivate(BasePlayer player, bool destroyUserInterface = true)
    {
        if (player == null)
            throw new NullReferenceException($"{nameof(player)} cannot be null!");

        Players.Remove(player.userID);

        if (destroyUserInterface)
            DeactivateUserInterface(player);
    }

    public virtual void Deactivate(IEnumerable<BasePlayer> players, bool destroyUserInterface = true)
    {
        foreach (BasePlayer player in players)
            Players.Remove(player.userID);

        if (destroyUserInterface)
            DeactivateUserInterface(players);
    }

    protected override void Update()
    {
        base.Update();

        TimeUserInterface = CreateTimeUserInterface(Minutes, Seconds);
        BroadcastTimeUserInterface(Players.Values);
    }

    protected virtual void ActivateUserInterface(BasePlayer player)
    {
        UserInterface?.AddUi(player);
        BroadcastTimeUserInterface(player);
    }

    protected virtual void ActivateUserInterface(IEnumerable<BasePlayer> players)
    {
        UserInterface?.AddUi(players);
        BroadcastTimeUserInterface(players);
    }

    protected virtual void DeactivateUserInterface(BasePlayer player)
    {
        UserInterface?.DestroyUi(player);
    }

    protected virtual void DeactivateUserInterface(IEnumerable<BasePlayer> players)
    {
        UserInterface?.DestroyUi(players);
    }

    protected virtual void BroadcastTimeUserInterface(BasePlayer player)
    {
        TimeUserInterface?.AddUi(player);
    }

    protected virtual void BroadcastTimeUserInterface(IEnumerable<BasePlayer> players)
    {
        TimeUserInterface?.AddUi(players);
    }

    protected abstract CachedUiBuilder CreateUserInterface();
    protected abstract CachedUiBuilder CreateTimeUserInterface(int minutes, int seconds);
}