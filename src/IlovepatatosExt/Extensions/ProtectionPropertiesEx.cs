using JetBrains.Annotations;
using UnityEngine;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class ProtectionPropertiesEx
{
    extension(ProtectionProperties property)
    {
        /// <summary>
        /// Returns a new instance of <see cref="ProtectionProperties"/> with the same values as the given property.
        /// </summary>
        [MustUseReturnValue]
        public ProtectionProperties Clone()
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
        public void SetAllValuesTo(float value)
        {
            for (int index = 0; index < property.amounts.Length; index++)
                property.amounts[index] = value;
        }
    }
}