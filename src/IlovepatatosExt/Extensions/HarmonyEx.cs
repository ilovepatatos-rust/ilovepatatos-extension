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
        string result = TrySmartPatchInternal(harmony, method);

        if (!string.IsNullOrEmpty(result))
            OxideConsole.Log($"Harmony exception! {result}", OxideConsole.YELLOW);
    }

    /// <summary>
    /// Try to patch all methods within a type.
    /// </summary>
    public static void TrySmartPatch(this Harmony harmony, Type type)
    {
        foreach (MethodInfo method in type.GetMethods((BindingFlags)~0))
            TrySmartPatch(harmony, method);
    }

    private static string TrySmartPatchInternal(Harmony harmony, MethodInfo method)
    {
        if (method == null)
            return "Couldn't patch null method!";

        var harmonyPatch = method.GetCustomAttribute<HarmonyPatch>();
        if (harmonyPatch == null) return $"Couldn't patch {method.Name}, because it doesn't have any {nameof(HarmonyPatch)} attribute!";

        MethodInfo original = AccessTools.Method(harmonyPatch.info.declaringType, harmonyPatch.info.methodName, harmonyPatch.info.argumentTypes);
        var patch = new HarmonyMethod(method.DeclaringType, method.Name);

        try
        {
            if (method.HasAttribute(typeof(HarmonyPrefix)))
                harmony.Patch(original, patch);
            else if (method.HasAttribute(typeof(HarmonyPostfix)))
                harmony.Patch(original, null, patch);
            else if (method.HasAttribute(typeof(HarmonyTranspiler)))
                harmony.Patch(original, null, null, patch);
            else return $"Couldn't patch {method.Name}, because it doesn't have any Harmony attribute!";

            return string.Empty;
        }
        catch (Exception ex)
        {
            return $"Couldn't patch {method.Name}! {ex.Message}";
        }
    }
}