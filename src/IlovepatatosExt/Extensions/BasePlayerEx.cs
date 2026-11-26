using JetBrains.Annotations;
using Network;
#if CARBON
using Oxide.Core.Libraries.Covalence;
#endif
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

    extension(BasePlayer player)
    {
        [MustUseReturnValue]
        public bool IsSelf(BasePlayer other)
        {
            return other != null && player.userID == other.userID;
        }

        [MustUseReturnValue]
        public bool IsModerator()
        {
            return player.Connection != null && player.Connection.IsModerator();
        }

        public void Reply(string msg)
        {
#if CARBON
            IPlayer iPlayer = player.AsIPlayer();
            if (iPlayer != null)
                IPlayerEx.Reply(iPlayer, msg);
            else
                player.ChatMessage(msg);
#else
            player.IPlayer?.Reply(msg);
#endif
        }

        [Obsolete("Use " + nameof(Reply) + "() instead")]
        public void ReplyToPlayer(string msg)
        {
            Reply(player, msg);
        }

        [MustUseReturnValue]
        public bool HasAnyPerms(params string[] perms)
        {
#if CARBON
            IPlayer iPlayer = player.AsIPlayer();
            return iPlayer != null && iPlayer.HasAnyPerms(perms);
#else
            return player != null && player.IPlayer.HasAnyPerms(perms);
#endif
        }

        [MustUseReturnValue]
        public bool HasAnyPerms(bool warn, params string[] perms)
        {
#if CARBON
            IPlayer iPlayer = player.AsIPlayer();
            return iPlayer != null && iPlayer.HasAnyPerms(warn, perms);
#else
            return player != null && player.IPlayer.HasAnyPerms(warn, perms);
#endif
        }

        public void ChatMessageAsCopyable(string msg, ulong steam64 = 0)
        {
            player.Connection.ChatMessageAsCopyable(msg, steam64);
        }

        [MustUseReturnValue]
        public bool HasInventorySpaceFor(string shortname, int amount)
        {
            return player.inventory != null && player.inventory.HasInventorySpaceFor(shortname, amount);
        }

        public bool Cast(out Vector3 pos, float distance = float.PositiveInfinity, int layerMask = CAST_LAYER)
        {
            bool result = Physics.Raycast(player.eyes.HeadRay(), out RaycastHit hit, distance, layerMask);
            pos = hit.point;
            return result;
        }

        [MustUseReturnValue]
        public T CastFirstByType<T>(float distance = float.PositiveInfinity, int layer = CAST_LAYER) where T : BaseEntity
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
        public T CastFirstByType<T>(out RaycastHit hit, float distance = float.PositiveInfinity, int layer = CAST_LAYER)
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
        public BaseEntity CastEntity(float distance = float.PositiveInfinity, int layer = CAST_LAYER)
        {
            Physics.Raycast(player.eyes.HeadRay(), out RaycastHit hit, distance, layer);
            return hit.GetEntity();
        }

        public void AddUi(string json)
        {
            player.Connection.AddUi(json);
        }

        public void DestroyUi(string name)
        {
            player.Connection.DestroyUi(name);
        }
    }

    extension([NotNull] IEnumerable<BasePlayer> players)
    {
        [MustUseReturnValue]
        public IEnumerable<Connection> GetOnlineConnections()
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
    }

    extension(IEnumerable<BasePlayer> players)
    {
        [MustUseReturnValue]
        public List<Connection> GetOnlineConnectionsPooled()
        {
            return players.GetOnlineConnections().ToPooledList();
        }

        public void ChatMessage(string msg, ulong steam64 = 0)
        {
            if (string.IsNullOrEmpty(msg))
                return;

            List<Connection> connections = players.GetOnlineConnectionsPooled();
            connections.ChatMessage(msg, steam64);
            PoolUtility.Free(ref connections);
        }

        public void ChatMessageAsCopyable(string msg, ulong steam64 = 0)
        {
            if (string.IsNullOrEmpty(msg))
                return;

            List<Connection> connections = players.GetOnlineConnectionsPooled();
            connections.ChatMessageAsCopyable(msg, steam64);
            PoolUtility.Free(ref connections);
        }

        public void ShowToast(GameTip.Styles style, Translate.Phrase phrase, bool overlay = false, params string[] arguments)
        {
            if (string.IsNullOrEmpty(phrase.english))
                return;

            List<Connection> connections = players.GetOnlineConnectionsPooled();
            connections.ShowToast(style, phrase, overlay, arguments);
            PoolUtility.Free(ref connections);
        }

        /// <summary>
        /// Sends a sound effect to each player at their location.
        /// </summary>
        /// <remarks>
        /// Use <see cref="Effects"/> for a list of sound effects.
        /// </remarks>
        public void PlaySfx(string sfx)
        {
            foreach (BasePlayer player in players)
                player.PlaySfx(sfx);
        }

        public void AddUi(string json)
        {
            if (string.IsNullOrEmpty(json))
                return;

            List<Connection> connections = players.GetOnlineConnectionsPooled();
            connections.AddUi(json);
            PoolUtility.Free(ref connections);
        }

        public void DestroyUi(string name)
        {
            if (string.IsNullOrEmpty(name))
                return;

            List<Connection> connections = players.GetOnlineConnectionsPooled();
            connections.DestroyUi(name);
            PoolUtility.Free(ref connections);
        }
    }

    extension([NotNull] BasePlayer player)
    {
        /// <summary>
        /// Sends a sound effect to the player at the player's location.
        /// </summary>
        /// <remarks>
        /// Use <see cref="Effects"/> for a list of sound effects.
        /// </remarks>
        public void PlaySfx([NotNull] string sfx)
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
    }
}