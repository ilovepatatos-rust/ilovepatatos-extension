using System.Reflection;
using JetBrains.Annotations;
using Timer = Oxide.Core.Libraries.Timer;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class TimerInstanceEx
{
    private static readonly FieldInfo s_ExpiresAtField = typeof(Timer.TimerInstance)
        .GetField("ExpiresAt", BindingFlags.Instance | BindingFlags.NonPublic);

    public static float Remaining(this Timer.TimerInstance timer)
    {
        object value = s_ExpiresAtField?.GetValue(timer);
        return value is float remaining ? remaining : -1;
    }
}