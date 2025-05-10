using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using Timer = Oxide.Core.Libraries.Timer;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class TimerInstanceEx
{
    private static readonly FieldInfo s_expiresAtField = AccessTools.Field(typeof(Timer.TimerInstance), "ExpiresAt");

    [MustUseReturnValue]
    public static float Remaining(this Timer.TimerInstance timer)
    {
        if (timer == null)
            throw new ArgumentNullException(nameof(timer));

        return (float)s_expiresAtField.GetValue(timer);
    }
}