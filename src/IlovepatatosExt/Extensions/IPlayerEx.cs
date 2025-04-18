﻿using JetBrains.Annotations;
using Newtonsoft.Json;
using Oxide.Core.Libraries.Covalence;
using UnityEngine;

namespace Oxide.Ext.IlovepatatosExt;

// ReSharper disable once InconsistentNaming
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class IPlayerEx
{
    public static ulong UserId(this IPlayer iPlayer)
    {
        return ulong.TryParse(iPlayer.Id, out ulong userId) ? userId : 0;
    }

    public static BasePlayer ToBasePlayer(this IPlayer iPlayer)
    {
        return iPlayer.Object as BasePlayer;
    }

    public static void ReplyToPlayer(this IPlayer iPlayer, string msg)
    {
        if (string.IsNullOrEmpty(msg))
            return;

        var player = iPlayer?.ToBasePlayer();
        if (player == null)
        {
            Debug.Log(msg);
        }
        else
        {
            if (iPlayer.LastCommand == CommandType.Console)
            {
                player.ConsoleMessage(msg);
            }
            else
            {
                player.ChatMessage(msg);
            }
        }
    }

    public static bool HasAnyPerms(this IPlayer iPlayer, params string[] perms)
    {
        return iPlayer != null && perms.Any(iPlayer.HasPermission);
    }

    public static bool HasAnyPerms(this IPlayer iPlayer, bool warn, params string[] perms)
    {
        bool isPlayerNull = iPlayer == null;
        bool hasPerms = !isPlayerNull && perms.Any(iPlayer.HasPermission);

        if (warn && !hasPerms)
            iPlayer.ReplyToPlayer("You don't have permissions to access this command!");

        return hasPerms;
    }

    public static bool IsAdmin(this IPlayer iPlayer)
    {
        if (iPlayer.IsAdmin)
            return true;

        return iPlayer.Object is BasePlayer { IsAdmin: true };
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