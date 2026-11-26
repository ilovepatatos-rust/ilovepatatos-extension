using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class HarmonyEx
{
    extension(Harmony harmony)
    {
        /// <summary>
        /// Try to patch a method with a Harmony attribute.
        /// </summary>
        public void TrySmartPatch(MethodInfo method)
        {
            HarmonyUtility.TrySmartPatch(harmony, method);
        }

        /// <summary>
        /// Try to patch all methods within a type.
        /// </summary>
        public void TrySmartPatch(Type type)
        {
            foreach (MethodInfo method in type.GetMethods((BindingFlags)~0))
            {
                if (HarmonyUtility.IsHarmonyMethod(method))
                    HarmonyUtility.TrySmartPatch(harmony, method);
            }
        }
    }
}