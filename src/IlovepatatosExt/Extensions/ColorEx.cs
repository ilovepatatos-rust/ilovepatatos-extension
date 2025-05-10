using JetBrains.Annotations;
using UnityEngine;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class ColorEx
{
    [MustUseReturnValue]
    public static Vector3 ToVector3(this Color self)
    {
        return new Vector3(self.r, self.g, self.b);
    }

    [MustUseReturnValue]
    public static string ToCuiColor(this Color self, float alpha = 1)
    {
        return $"{self.r} {self.g} {self.b} {alpha}";
    }
}