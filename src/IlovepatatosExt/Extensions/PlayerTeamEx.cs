using JetBrains.Annotations;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class PlayerTeamEx
{
    extension(RelationshipManager.PlayerTeam team)
    {
        public void ChatMessage(string msg, ulong steam64 = 0)
        {
            team.GetOnlineMemberConnections().ChatMessage(msg, steam64);
        }
    }
}