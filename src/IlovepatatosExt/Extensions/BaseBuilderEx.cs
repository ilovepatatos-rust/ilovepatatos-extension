using JetBrains.Annotations;
using Oxide.Ext.UiFramework.Builder;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class BaseBuilderEx
{
    public static void AddUi(this BaseBuilder builder, IEnumerable<BasePlayer> players)
    {
        var connections = players.GetOnlineConnectionsPooled();
        builder.AddUi(connections);
        PoolUtility.Free(ref connections);
    }

    public static void DestroyUi(this BaseBuilder builder, IEnumerable<BasePlayer> players)
    {
        var connections = players.GetOnlineConnectionsPooled();
        builder.DestroyUi(connections);
        PoolUtility.Free(ref connections);
    }
}