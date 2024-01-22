using JetBrains.Annotations;
using Network;
using UnityEngine;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class BasePlayerEx
{
    public static void ReplyToPlayer(this BasePlayer player, string msg)
    {
        player.IPlayer?.ReplyToPlayer(msg);
    }

    public static IEnumerable<Connection> GetOnlineConnections(this IEnumerable<BasePlayer> players)
    {
        return from player in players where player != null select player.Connection into con where con != null select con;
    }

    public static List<Connection> GetOnlineConnectionsPooled(this IEnumerable<BasePlayer> players)
    {
        return players.GetOnlineConnections().ToPooledList();
    }

    public static void ChatMessage(this IEnumerable<BasePlayer> players, string msg, ulong steam64 = 0)
    {
        if (string.IsNullOrEmpty(msg))
            return;

        List<Connection> connections = players.GetOnlineConnectionsPooled();
        connections.ChatMessage(msg, steam64);
        PoolUtility.Free(ref connections);
    }

    public static void ShowToast(this IEnumerable<BasePlayer> players, GameTip.Styles style, Translate.Phrase phrase, params string[] arguments)
    {
        if (string.IsNullOrEmpty(phrase.english))
            return;

        List<Connection> connections = players.GetOnlineConnectionsPooled();
        connections.ShowToast(style, phrase, arguments);
        PoolUtility.Free(ref connections);
    }

    public static bool Cast(this BasePlayer player, out Vector3 pos, float distance = float.PositiveInfinity, int layerMask = -1)
    {
        bool result = Physics.Raycast(player.eyes.HeadRay(), out RaycastHit hit, distance, layerMask);
        pos = hit.point;
        return result;
    }

    public static BaseEntity CastEntity(this BasePlayer player, float distance = float.PositiveInfinity, int layer = -1)
    {
        Physics.Raycast(player.eyes.HeadRay(), out RaycastHit hit, distance, layer);
        return hit.GetEntity();
    }
}