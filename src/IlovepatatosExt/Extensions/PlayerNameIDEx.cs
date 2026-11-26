using JetBrains.Annotations;
using ProtoBuf;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class PlayerNameIDEx
{
    extension(PlayerNameID playerNameID)
    {
        [MustUseReturnValue]
        public BasePlayer ToBasePlayer()
        {
            return BasePlayer.FindByID(playerNameID.userid);
        }

        [MustUseReturnValue]
        public BasePlayer ToBasePlayerAwakeOrSleeping()
        {
            return BasePlayer.FindAwakeOrSleeping($"{playerNameID.userid}");
        }

        [MustUseReturnValue]
        public bool IsOnline()
        {
            BasePlayer player = ToBasePlayer(playerNameID);
            return player != null && player.IsConnected;
        }
    }
}