using JetBrains.Annotations;
using UnityEngine;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class ComponentEx
{
    public static bool TryDestroyComponent<T>(this Component obj, bool immediate = false) where T : Component
    {
        var comp = obj.GetComponent<T>();
        if (comp == null) return false;

        if (immediate)
        {
            UnityEngine.Object.DestroyImmediate(comp);
        }
        else
        {
            UnityEngine.Object.Destroy(comp);
        }

        return true;
    }
}