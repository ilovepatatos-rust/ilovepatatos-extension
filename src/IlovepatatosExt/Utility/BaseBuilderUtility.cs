using JetBrains.Annotations;
using Network;
using Oxide.Ext.UiFramework.Builder;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class BaseBuilderUtility
{
    public static void DestroyUi(IEnumerable<BasePlayer> players, string name)
    {
        var connections = players.GetOnlineConnectionsPooled();
        var info = new SendInfo(connections);

        BaseBuilder.DestroyUi(info, name);
        PoolUtility.Free(ref connections);
    }
}