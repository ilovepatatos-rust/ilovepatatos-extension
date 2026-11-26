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
    [MustUseReturnValue]
    public static ulong UserId(this IPlayer iPlayer)
    {
        return ulong.TryParse(iPlayer.Id, out ulong userId) ? userId : 0;
    }

    [MustUseReturnValue]
    public static BasePlayer ToBasePlayer(this IPlayer iPlayer)
    {
        return iPlayer.Object as BasePlayer;
    }

    [Obsolete("Use " + nameof(IPlayer.Reply) + "() instead")]
    public static void ReplyToPlayer(this IPlayer iPlayer, string msg)
    {
        Reply(iPlayer, msg);
    }

    /// <summary>
    /// Replies like Oxide: chat if the last command was chat, console if it was console.
    /// Do not call <see cref="IPlayer.Reply(string)"/> on Carbon — it always writes to chat.
    /// </summary>
    public static void Reply(this IPlayer iPlayer, string msg)
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
    public static bool HasAnyPerms(this IPlayer iPlayer, params string[] perms)
    {
        return iPlayer != null && perms.Any(perm => HasPermission(iPlayer, perm));
    }

    [MustUseReturnValue]
    public static bool HasAnyPerms(this IPlayer iPlayer, bool warn, params string[] perms)
    {
        if (iPlayer == null)
            return false;

        bool hasPerms = perms.Any(perm => HasPermission(iPlayer, perm));
        if (warn && !hasPerms)
            Reply(iPlayer, "You don't have permissions to access this command!");

        return hasPerms;
    }

    [Obsolete("Use " + nameof(IsAdminOrModerator) + "() instead")]
    public static bool IsAdmin(this IPlayer iPlayer)
    {
        return IsAdminOrModerator(iPlayer);
    }

    [MustUseReturnValue]
    public static bool IsAdminOrModerator(this IPlayer iPlayer)
    {
        return iPlayer.Object is BasePlayer { IsAdmin: true } || iPlayer.IsAdmin;
    }

    public static void ReplyWithObject(this IPlayer iPlayer, object obj, Formatting formatting = Formatting.Indented)
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
