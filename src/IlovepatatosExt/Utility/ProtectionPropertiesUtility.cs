using JetBrains.Annotations;
using Rust;
using UnityEngine;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class ProtectionPropertiesUtility
{
    public static readonly ProtectionProperties Invincible = CreateProtectionProperties(1f);

    public static ProtectionProperties CreateProtectionProperties(float fallback, params KeyValuePair<DamageType, float>[] protections)
    {
        var instance = ScriptableObject.CreateInstance<ProtectionProperties>();

        for (int i = 0; i < instance.amounts.Length; i++)
            instance.amounts[i] = fallback;

        foreach (KeyValuePair<DamageType, float> value in protections)
            instance.amounts[(int)value.Key] = value.Value;

        return instance;
    }
}