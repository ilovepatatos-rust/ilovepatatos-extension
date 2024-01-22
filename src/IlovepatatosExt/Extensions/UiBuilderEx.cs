using JetBrains.Annotations;
using Oxide.Ext.UiFramework.Builder.UI;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class UiBuilderEx
{
    public static void AddUi(this UiBuilder builder, IEnumerable<BasePlayer> players)
    {
        var connections = players.GetOnlineConnectionsPooled();
        builder.AddUi(connections);
        PoolUtility.Free(ref connections);
    }

    public static void DestroyUi(this UiBuilder builder, IEnumerable<BasePlayer> players)
    {
        var connections = players.GetOnlineConnectionsPooled();
        builder.DestroyUi(connections);
        PoolUtility.Free(ref connections);
    }
}