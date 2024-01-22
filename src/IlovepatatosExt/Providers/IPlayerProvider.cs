using JetBrains.Annotations;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly]
public interface IPlayerProvider
{
    IEnumerable<BasePlayer> GetPlayers();
}