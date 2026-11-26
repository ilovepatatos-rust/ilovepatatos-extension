using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using Timer = Oxide.Core.Libraries.Timer;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class TimerInstanceEx
{
    private static readonly FieldInfo s_expiresAtField = AccessTools.Field(typeof(Timer.TimerInstance), "ExpiresAt");

    extension(Timer.TimerInstance timer)
    {
        [MustUseReturnValue]
        public float Remaining()
        {
            if (timer == null)
                throw new ArgumentNullException(nameof(timer));

            return (float)s_expiresAtField.GetValue(timer);
        }
    }
}