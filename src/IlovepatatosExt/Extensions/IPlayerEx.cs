using JetBrains.Annotations;
using Newtonsoft.Json;
using Oxide.Core.Libraries.Covalence;

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
        iPlayer.Reply(msg);
    }

    [MustUseReturnValue]
    public static bool HasAnyPerms(this IPlayer iPlayer, params string[] perms)
    {
        return iPlayer != null && perms.Any(iPlayer.HasPermission);
    }

    [MustUseReturnValue]
    public static bool HasAnyPerms(this IPlayer iPlayer, bool warn, params string[] perms)
    {
        if (iPlayer == null)
            return false;

        bool hasPerms = perms.Any(iPlayer.HasPermission);
        if (warn && !hasPerms)
            iPlayer.Reply("You don't have permissions to access this command!");

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
            iPlayer?.Reply(value);
        }
        else if (obj != null)
        {
            string json = JsonConvert.SerializeObject(obj, formatting);
            iPlayer?.Reply(json);
        }
    }
}