using JetBrains.Annotations;
using UnityEngine;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class ColorUtility
{
    public static Color ParseHexString(string hex)
    {
        UnityEngine.ColorUtility.TryParseHtmlString(hex, out Color color);
        return color;
    }
}