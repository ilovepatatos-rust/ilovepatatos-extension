using JetBrains.Annotations;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly]
public class ServerPlayersProvider : IPlayerProvider
{
    public IEnumerable<BasePlayer> GetPlayers()
    {
        return BasePlayer.activePlayerList;
    }
}