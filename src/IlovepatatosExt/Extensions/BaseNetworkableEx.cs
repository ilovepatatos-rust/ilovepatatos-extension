using JetBrains.Annotations;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class BaseNetworkableEx
{
    extension(BaseNetworkable networkable)
    {
        public void KillOnce(BaseNetworkable.DestroyMode mode = BaseNetworkable.DestroyMode.None)
        {
            if (networkable != null && !networkable.IsDestroyed)
                networkable.Kill(mode);
        }
    }

    extension(IEnumerable<BaseNetworkable> networkables)
    {
        public void KillAll(BaseNetworkable.DestroyMode mode = BaseNetworkable.DestroyMode.None)
        {
            foreach (BaseNetworkable networkable in networkables)
                networkable.KillOnce(mode);
        }
    }
}