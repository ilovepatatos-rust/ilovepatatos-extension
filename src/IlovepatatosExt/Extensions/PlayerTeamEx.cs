using JetBrains.Annotations;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class PlayerTeamEx
{
    public static void ChatMessage(this RelationshipManager.PlayerTeam team, string msg, ulong steam64 = 0)
    {
        team.GetOnlineMemberConnections().ChatMessage(msg, steam64);
    }
}