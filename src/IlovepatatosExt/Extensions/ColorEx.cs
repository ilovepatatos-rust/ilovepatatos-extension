using JetBrains.Annotations;
using UnityEngine;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class ColorEx
{
    extension(Color self)
    {
        [MustUseReturnValue]
        public Vector3 ToVector3()
        {
            return new Vector3(self.r, self.g, self.b);
        }

        [MustUseReturnValue]
        public string ToCuiColor(float alpha = 1)
        {
            return $"{self.r} {self.g} {self.b} {alpha}";
        }
    }
}