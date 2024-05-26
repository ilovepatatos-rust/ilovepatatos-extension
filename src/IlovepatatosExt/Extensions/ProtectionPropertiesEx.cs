using JetBrains.Annotations;
using UnityEngine;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class ProtectionPropertiesEx
{
    /// <summary>
    /// Returns a new instance of <see cref="ProtectionProperties"/> with the same values as the given property.
    /// </summary>
    public static ProtectionProperties Clone(this ProtectionProperties property)
    {
        var instance = ScriptableObject.CreateInstance<ProtectionProperties>();
        instance.comments = property.comments;
        instance.density = property.density;

        for (int index = 0; index < instance.amounts.Length && index < property.amounts.Length; index++)
            instance.amounts[index] = property.amounts[index];

        return instance;
    }

    /// <summary>
    /// Set all damage types to a given value.
    /// </summary>
    public static void SetAllValuesTo(this ProtectionProperties property, float value)
    {
        for (int index = 0; index < property.amounts.Length; index++)
            property.amounts[index] = value;
    }
}