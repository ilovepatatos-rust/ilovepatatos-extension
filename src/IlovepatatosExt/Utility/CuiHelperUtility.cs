using JetBrains.Annotations;
using Network;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class CuiHelperUtility
{
    private static void BroadcastAction(List<Connection> connections, string action, string arg)
    {
        if (connections.Count > 0)
            CommunityEntity.ServerInstance.ClientRPCEx(new SendInfo(connections), null, action, arg);
    }

    public static void AddUi(IEnumerable<BasePlayer> players, string json)
    {
        var connections = players.GetOnlineConnectionsPooled();
        BroadcastAction(connections, json, "AddUI");
        PoolUtility.Free(ref connections);
    }

    public static void DestroyUi(IEnumerable<BasePlayer> players, string name)
    {
        var connections = players.GetOnlineConnectionsPooled();
        BroadcastAction(connections, name, "DestroyUI");
        PoolUtility.Free(ref connections);
    }
}