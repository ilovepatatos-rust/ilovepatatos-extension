using System.Reflection;
using JetBrains.Annotations;
using Network;
using Rust;
using UnityEngine;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class BasePlayerEx
{
    private static readonly FieldInfo s_playerNonSuicideInfoField =
        typeof(BasePlayer).GetField("cachedNonSuicideHitInfo", BindingFlags.Instance | BindingFlags.NonPublic);

    public static bool IsSelf(this BasePlayer player, BasePlayer other)
    {
        return other != null && player.userID == other.userID;
    }

    public static bool IsModerator(this BasePlayer player)
    {
        return player.Connection != null && player.Connection.IsModerator();
    }

    public static void ReplyToPlayer(this BasePlayer player, string msg)
    {
        player.IPlayer?.ReplyToPlayer(msg);
    }

    public static bool HasAnyPerms(this BasePlayer player, params string[] perms)
    {
        return player != null && player.IPlayer.HasAnyPerms(perms);
    }

    public static bool HasAnyPerms(this BasePlayer player, bool warn, params string[] perms)
    {
        return player != null && player.IPlayer.HasAnyPerms(warn, perms);
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

    public static void ChatMessageAsCopyable(this IEnumerable<BasePlayer> players, string msg, ulong steam64 = 0)
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

    public static bool HasInventorySpaceFor(this BasePlayer player, string shortname, int amount)
    {
        return player.inventory != null && player.inventory.HasInventorySpaceFor(shortname, amount);
    }

    /// <summary>
    /// Returns the last <see cref="HitInfo"/> that caused the player to die or wound.
    /// Use this to get the actual killer when the player is wounded and f1 kills.
    /// </summary>
    public static HitInfo GetActualDyingInfo(this BasePlayer player, HitInfo info)
    {
        if (!player.IsWounded() || player.lastDamage == DamageType.Suicide)
            return info;

        var other = s_playerNonSuicideInfoField?.GetValue(player) as HitInfo;
        return other ?? info;
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

    public static void AddUi(this BasePlayer player, string json)
    {
        player.Connection.AddUi(json);
    }

    public static void AddUi(this IEnumerable<BasePlayer> players, string json)
    {
        if (string.IsNullOrEmpty(json))
            return;

        List<Connection> connections = players.GetOnlineConnectionsPooled();
        connections.AddUi(json);
        PoolUtility.Free(ref connections);
    }

    public static void DestroyUi(this BasePlayer player, string name)
    {
        player.Connection.DestroyUi(name);
    }

    public static void DestroyUi(this IEnumerable<BasePlayer> players, string name)
    {
        if (string.IsNullOrEmpty(name))
            return;

        List<Connection> connections = players.GetOnlineConnectionsPooled();
        connections.DestroyUi(name);
        PoolUtility.Free(ref connections);
    }
}