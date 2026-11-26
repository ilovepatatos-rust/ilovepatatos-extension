using JetBrains.Annotations;
using Oxide.Ext.UiFramework.Builder;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class BaseBuilderEx
{
    extension(BaseBuilder builder)
    {
        public void AddUi(IEnumerable<BasePlayer> players)
        {
            var connections = players.GetOnlineConnectionsPooled();
            builder.AddUi(connections);
            PoolUtility.Free(ref connections);
        }

        public void DestroyUi(IEnumerable<BasePlayer> players)
        {
            var connections = players.GetOnlineConnectionsPooled();
            builder.DestroyUi(connections);
            PoolUtility.Free(ref connections);
        }
    }
}