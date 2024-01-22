using JetBrains.Annotations;
using Oxide.Core;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class TimerUtility
{
    public static readonly Core.Libraries.Timer TimersPool = Interface.Oxide.GetLibrary<Core.Libraries.Timer>("Timer");

    public static void DestroyToPool(ref Plugins.Timer timer)
    {
        timer?.DestroyToPool();
        timer = null;
    }

    public static void DestroyToPool(ref Core.Libraries.Timer.TimerInstance timer)
    {
        timer?.DestroyToPool();
        timer = null;
    }
}