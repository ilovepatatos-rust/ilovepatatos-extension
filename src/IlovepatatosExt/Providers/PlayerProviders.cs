using Facepunch;
using JetBrains.Annotations;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class PlayerProviders : IPlayerProvider, Pool.IPooled
{
    private List<IPlayerProvider> _providers;

    public static PlayerProviders New(params IPlayerProvider[] providers)
    {
        var provider = PoolUtility.Get<PlayerProviders>();
        provider._providers.AddRange(providers);

        return provider;
    }

    public IEnumerable<BasePlayer> GetPlayers()
    {
        var done = PoolUtility.Get<HashSet<ulong>>();

        foreach (IPlayerProvider provider in _providers)
        {
            foreach (BasePlayer player in provider.GetPlayers())
            {
                if (player != null && done.Add(player.userID))
                    yield return player;
            }
        }

        PoolUtility.Free(ref done);
    }

    void Pool.IPooled.EnterPool()
    {
        PoolUtility.Free(ref _providers);
    }

    void Pool.IPooled.LeavePool()
    {
        _providers = PoolUtility.Get<List<IPlayerProvider>>();
    }
}