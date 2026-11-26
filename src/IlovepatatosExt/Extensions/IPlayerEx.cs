using JetBrains.Annotations;
using Newtonsoft.Json;
using Oxide.Core.Libraries.Covalence;
using Oxide.Game.Rust.Libraries.Covalence;
#if CARBON
using Oxide.Core;
using Oxide.Core.Libraries;
#endif

namespace Oxide.Ext.IlovepatatosExt;

// ReSharper disable once InconsistentNaming
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class IPlayerEx
{
    extension(IPlayer iPlayer)
    {
        [MustUseReturnValue]
        public ulong UserId()
        {
            return ulong.TryParse(iPlayer.Id, out ulong userId) ? userId : 0;
        }

        [MustUseReturnValue]
        public BasePlayer ToBasePlayer()
        {
            return iPlayer.Object as BasePlayer;
        }

        [Obsolete("Use " + nameof(IPlayer.Reply) + "() instead")]
        public void ReplyToPlayer(string msg)
        {
            Reply(iPlayer, msg);
        }

        /// <summary>
        /// Replies like Oxide: chat if the last command was chat, console if it was console.
        /// Do not call <see cref="IPlayer.Reply(string)"/> on Carbon — it always writes to chat.
        /// </summary>
        public void Reply(string msg)
        {
            if (iPlayer == null || string.IsNullOrEmpty(msg))
                return;

#if CARBON
            if (iPlayer.LastCommand == CommandType.Console)
            {
                if (iPlayer.Object is BasePlayer consolePlayer)
                {
                    consolePlayer.ConsoleMessage(Formatter.ToPlaintext(msg));
                    return;
                }
            }

            if (iPlayer.Object is BasePlayer chatPlayer)
            {
                chatPlayer.ChatMessage(msg);
                return;
            }

            iPlayer.Message(msg);
#else
            iPlayer.Reply(msg);
#endif
        }

        [MustUseReturnValue]
        public bool HasAnyPerms(params string[] perms)
        {
            return iPlayer != null && perms.Any(perm => HasPermission(iPlayer, perm));
        }

        [MustUseReturnValue]
        public bool HasAnyPerms(bool warn, params string[] perms)
        {
            if (iPlayer == null)
                return false;

            bool hasPerms = perms.Any(perm => HasPermission(iPlayer, perm));
            if (warn && !hasPerms)
                Reply(iPlayer, "You don't have permissions to access this command!");

            return hasPerms;
        }

        [Obsolete("Use " + nameof(IsAdminOrModerator) + "() instead")]
        public bool IsAdmin()
        {
            return IsAdminOrModerator(iPlayer);
        }

        [MustUseReturnValue]
        public bool IsAdminOrModerator()
        {
            return iPlayer.Object is BasePlayer { IsAdmin: true } || iPlayer.IsAdmin;
        }

        public void ReplyWithObject(object obj, Formatting formatting = Formatting.Indented)
        {
            if (obj is string value)
            {
                Reply(iPlayer, value);
            }
            else if (obj != null)
            {
                string json = JsonConvert.SerializeObject(obj, formatting);
                Reply(iPlayer, json);
            }
        }
    }

    [MustUseReturnValue]
    private static bool HasPermission(IPlayer iPlayer, string perm)
    {
#if CARBON
        // Carbon's IPlayer.HasPermission returns true when IsServer defaults to true.
        Permission permission = Interface.Oxide.GetLibrary<Permission>();
        return permission != null && permission.UserHasPermission(iPlayer.Id, perm);
#else
        return iPlayer.HasPermission(perm);
#endif
    }
}
