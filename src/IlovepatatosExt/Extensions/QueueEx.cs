using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class QueueEx
{
    [MustUseReturnValue]
    public static int Capacity<T>(this Queue<T> queue)
    {
        FieldInfo field = AccessTools.Field(queue.GetType(), "_array");
        var array = (T[])field.GetValue(queue);

        return array.Length;
    }
}