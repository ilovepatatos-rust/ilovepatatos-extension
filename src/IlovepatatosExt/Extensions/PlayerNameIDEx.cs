using JetBrains.Annotations;
using ProtoBuf;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class PlayerNameIDEx
{
    [MustUseReturnValue]
    public static BasePlayer ToBasePlayer(this PlayerNameID playerNameID)
    {
        return BasePlayer.FindByID(playerNameID.userid);
    }

    [MustUseReturnValue]
    public static BasePlayer ToBasePlayerAwakeOrSleeping(this PlayerNameID playerNameID)
    {
        return BasePlayer.FindAwakeOrSleeping($"{playerNameID.userid}");
    }

    [MustUseReturnValue]
    public static bool IsOnline(this PlayerNameID playerNameID)
    {
        BasePlayer player = ToBasePlayer(playerNameID);
        return player != null && player.IsConnected;
    }
}