using JetBrains.Annotations;
using Network;
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
                                   1 << (int)Layer.World |
                                   1 << (int)Layer.Player_Server |
                                   1 << (int)Layer.Construction |
                                   1 << (int)Layer.Terrain |
                                   1 << (int)Layer.Tree;

    private static readonly RaycastHit[] s_results = new RaycastHit[128];

    private static readonly Effect s_reusableEffect = new();

    [MustUseReturnValue]
    public static bool IsSelf(this BasePlayer player, BasePlayer other)
    {
        return other != null && player.userID == other.userID;
    }

    [MustUseReturnValue]
    public static bool IsModerator(this BasePlayer player)
    {
        return player.Connection != null && player.Connection.IsModerator();
    }

    public static void Reply(this BasePlayer player, string msg)
    {
        player.IPlayer?.Reply(msg);
    }

    [Obsolete("Use " + nameof(Reply) + "() instead")]
    public static void ReplyToPlayer(this BasePlayer player, string msg)
    {
        Reply(player, msg);
    }

    [MustUseReturnValue]
    public static bool HasAnyPerms(this BasePlayer player, params string[] perms)
    {
        return player != null && player.IPlayer.HasAnyPerms(perms);
    }

    [MustUseReturnValue]
    public static bool HasAnyPerms(this BasePlayer player, bool warn, params string[] perms)
    {
        return player != null && player.IPlayer.HasAnyPerms(warn, perms);
    }

    [MustUseReturnValue]
    public static IEnumerable<Connection> GetOnlineConnections([NotNull] this IEnumerable<BasePlayer> players)
    {
        if (players == null)
            throw new ArgumentNullException(nameof(players));

        foreach (BasePlayer player in players)
        {
            if (player == null)
                continue;

            Connection connection = player.Connection;
            if (connection == null)
                continue;

            yield return connection;
        }
    }

    [MustUseReturnValue]
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

    [MustUseReturnValue]
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
    public static void PlaySfx([NotNull] this BasePlayer player, [NotNull] string sfx)
    {
        if (player == null)
            throw new ArgumentNullException(nameof(player));

        if (string.IsNullOrEmpty(sfx))
            throw new ArgumentNullException(nameof(sfx));

        Effect effect = s_reusableEffect;
        effect.Init(Effect.Type.Generic, player, 0, Vector3.zero, Vector3.forward);
        effect.pooledString = sfx;

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
    public static T CastFirstByType<T>(this BasePlayer player, float distance = float.PositiveInfinity, int layer = CAST_LAYER) where T : BaseEntity
    {
        Ray origin = player.eyes.HeadRay();
        int size = Physics.RaycastNonAlloc(origin, s_results, distance, layer);

        for (int i = 0; i < size; i++)
        {
            Collider collider = s_results[i].collider;
            if (collider == null)
                continue;

            var entity = collider.ToBaseEntity();
            if (entity is T value)
                return value;
        }

        return null;
    }

    [MustUseReturnValue]
    public static T CastFirstByType<T>(this BasePlayer player, out RaycastHit hit, float distance = float.PositiveInfinity, int layer = CAST_LAYER)
        where T : BaseEntity
    {
        Ray origin = player.eyes.HeadRay();
        int size = Physics.RaycastNonAlloc(origin, s_results, distance, layer);

        for (int i = 0; i < size; i++)
        {
            hit = s_results[i];

            Collider collider = hit.collider;
            if (collider == null)
                continue;

            var entity = collider.ToBaseEntity();
            if (entity is T value)
                return value;
        }

        hit = default;
        return null;
    }

    [MustUseReturnValue]
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