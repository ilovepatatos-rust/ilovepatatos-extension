using System.Diagnostics;
using System.Reflection;
using Facepunch.Extend;
using HarmonyLib;
using JetBrains.Annotations;
using Oxide.Ext.ConsoleExt;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class HarmonyEx
{
    /// <summary>
    /// Try to patch a method with a Harmony attribute.
    /// </summary>
    public static void TrySmartPatch(this Harmony harmony, MethodInfo method)
    {
        HarmonyUtility.TrySmartPatch(harmony, method);
    }

    /// <summary>
    /// Try to patch all methods within a type.
    /// </summary>
    public static void TrySmartPatch(this Harmony harmony, Type type)
    {
        foreach (MethodInfo method in type.GetMethods((BindingFlags)~0))
        {
            if (HarmonyUtility.IsHarmonyMethod(method))
                HarmonyUtility.TrySmartPatch(harmony, method);
        }
    }


}