using JetBrains.Annotations;
using Network;
using Oxide.Core.Libraries.Covalence;
using Rust;
using UnityEngine;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class BasePlayerEx
{
    private const int CAST_LAYER = 1 << (int)Layer.Default |
                                   1 << (int)Layer.Deployed |
                                   1 << (int)Layer.Ragdoll |
                                   1 << (int)Layer.AI |
                                   1 << (int)Layer.Player_Server |
                                   1 << (int)Layer.Construction |
                                   1 << (int)Layer.Terrain |
                                   1 << (int)Layer.Tree;

    private static readonly RaycastHit[] s_results = new RaycastHit[128];

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
        IPlayer iPlayer = player == null ? null : player.IPlayer;
        iPlayer?.ReplyToPlayer(msg);
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

    public static void ChatMessageAsCopyable(this BasePlayer player, string msg, ulong steam64 = 0)
    {
        player.Connection.ChatMessageAsCopyable(msg, steam64);
    }

    public static void ChatMessageAsCopyable(this IEnumerable<BasePlayer> players, string msg, ulong steam64 = 0)
    {
        if (string.IsNullOrEmpty(msg))
            return;

        List<Connection> connections = players.GetOnlineConnectionsPooled();
        connections.ChatMessageAsCopyable(msg, steam64);
        PoolUtility.Free(ref connections);
    }

    public static void ShowToast(this IEnumerable<BasePlayer> players,
        GameTip.Styles style, Translate.Phrase phrase, bool overlay = false, params string[] arguments)
    {
        if (string.IsNullOrEmpty(phrase.english))
            return;

        List<Connection> connections = players.GetOnlineConnectionsPooled();
        connections.ShowToast(style, phrase, overlay, arguments);
        PoolUtility.Free(ref connections);
    }

    public static bool HasInventorySpaceFor(this BasePlayer player, string shortname, int amount)
    {
        return player.inventory != null && player.inventory.HasInventorySpaceFor(shortname, amount);
    }

    /// <summary>
    /// Sends a sound effect to the player at the player's location.
    /// </summary>
    /// <remarks>
    /// Use <see cref="Effects"/> for a list of sound effects.
    /// </remarks>
    public static void PlaySfx(this BasePlayer player, string sfx)
    {
        var effect = new Effect(sfx, player, 0, Vector3.zero, Vector3.forward);
        EffectNetwork.Send(effect, player.Connection);
    }

    /// <summary>
    /// Sends a sound effect to each player at their location.
    /// </summary>
    /// <remarks>
    /// Use <see cref="Effects"/> for a list of sound effects.
    /// </remarks>
    public static void PlaySfx(this IEnumerable<BasePlayer> players, string sfx)
    {
        foreach (BasePlayer player in players)
            player.PlaySfx(sfx);
    }

    public static bool Cast(this BasePlayer player, out Vector3 pos, float distance = float.PositiveInfinity, int layerMask = CAST_LAYER)
    {
        bool result = Physics.Raycast(player.eyes.HeadRay(), out RaycastHit hit, distance, layerMask);
        pos = hit.point;
        return result;
    }

    [MustUseReturnValue]
    public static T Cast<T>(this BasePlayer player, float distance = float.PositiveInfinity, int layer = CAST_LAYER) where T : Component
    {
        Ray origin = player.eyes.HeadRay();
        int size = Physics.RaycastNonAlloc(origin, s_results, distance, layer);

        for (int i = 0; i < size; i++)
        {
            Collider collider = s_results[i].collider;
            if (collider == null)
                continue;

            GameObject go = collider.gameObject;
            if (go == null)
                continue;

            var component = go.GetComponent<T>();
            if (component != null)
                return component;
        }

        return null;
    }

    public static BaseEntity CastEntity(this BasePlayer player, float distance = float.PositiveInfinity, int layer = CAST_LAYER)
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