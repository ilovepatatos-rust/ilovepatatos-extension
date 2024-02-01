using JetBrains.Annotations;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class BaseNetworkableEx
{
    public static void KillOnce(this BaseNetworkable networkable, BaseNetworkable.DestroyMode mode = BaseNetworkable.DestroyMode.None)
    {
        if (networkable != null && !networkable.IsDestroyed)
            networkable.Kill(mode);
    }

    public static void KillAll(this IEnumerable<BaseNetworkable> networkables, BaseNetworkable.DestroyMode mode = BaseNetworkable.DestroyMode.None)
    {
        foreach (BaseNetworkable networkable in networkables)
            networkable.KillOnce(mode);
    }
}