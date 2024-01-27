using JetBrains.Annotations;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class CuiHelperUtility
{
    public static void AddUi(IEnumerable<BasePlayer> players, string json) => players.AddUi(json);
    public static void DestroyUi(IEnumerable<BasePlayer> players, string name) => players.DestroyUi(name);
}