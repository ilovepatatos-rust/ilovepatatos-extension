using Facepunch;
using JetBrains.Annotations;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class PlayerProviders : IPlayerProvider, Pool.IPooled
{
    private readonly List<IPlayerProvider> _providers = new();

    /// <summary>
    /// Since we can't use pool list while yielding in case it breaks/returns.
    /// We'll assume GetPlayers() won't be called by multiple systems async and use an internal field instead.
    /// </summary>
    private HashSet<ulong> _playersReturned;

    public static PlayerProviders New(params IPlayerProvider[] providers)
    {
        var provider = PoolUtility.Get<PlayerProviders>();
        provider._providers.AddRange(providers);

        return provider;
    }

    public IEnumerable<BasePlayer> GetPlayers()
    {
        _playersReturned.Clear();

        foreach (IPlayerProvider provider in _providers)
        {
            foreach (BasePlayer player in provider.GetPlayers())
            {
                if (player != null && _playersReturned.Add(player.userID))
                    yield return player;
            }
        }
    }

    void Pool.IPooled.EnterPool()
    {
        _providers.Clear();
        PoolUtility.Free(ref _playersReturned);
    }

    void Pool.IPooled.LeavePool()
    {
        _playersReturned = PoolUtility.Get<HashSet<ulong>>();
    }
}