using System.Diagnostics;
using System.Reflection;
using Facepunch.Extend;
using HarmonyLib;
using Oxide.Ext.ConsoleExt;

namespace Oxide.Ext.IlovepatatosExt;

internal static class HarmonyUtility
{
    internal static bool IsHarmonyMethod(MethodInfo method)
    {
        return method.HasAttribute(typeof(HarmonyPrefix)) ||
               method.HasAttribute(typeof(HarmonyPostfix)) ||
               method.HasAttribute(typeof(HarmonyTranspiler));
    }

    internal static void TrySmartPatch(this Harmony harmony, MethodInfo method)
    {
        string result = TrySmartPatchInternal(harmony, method);
        if (string.IsNullOrEmpty(result)) return;

        StackTrace stackTrace = new(2, false); // skip itself & HarmonyEx
        StackFrame frame = stackTrace.GetFrame(0);

        Type type = frame?.GetMethod().DeclaringType;
        string caller = type?.Name ?? "Unknown";

        OxideConsole.Log($"[{caller}] Harmony exception! {result}", OxideConsole.YELLOW);
    }

    private static string TrySmartPatchInternal(Harmony harmony, MethodInfo method)
    {
        if (method == null)
            return "Couldn't patch null method!";

        var harmonyPatch = method.GetCustomAttribute<HarmonyPatch>();
        if (harmonyPatch == null) return $"Couldn't patch {method.Name}, because it doesn't have any {nameof(HarmonyPatch)} attribute!";

        try
        {
            MethodInfo original = GetOriginalMethod(harmonyPatch);
            HarmonyMethod patch = method;

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
            return $"Couldn't patch {method.Name}! {ex}";
        }
    }

    private static MethodInfo GetOriginalMethod(HarmonyPatch patch)
    {
        return patch.info.methodType switch
        {
            MethodType.Getter => AccessTools.PropertyGetter(patch.info.declaringType, patch.info.methodName),
            MethodType.Setter => AccessTools.PropertySetter(patch.info.declaringType, patch.info.methodName),
            _ => AccessTools.Method(patch.info.declaringType, patch.info.methodName, patch.info.argumentTypes)
        };
    }
}