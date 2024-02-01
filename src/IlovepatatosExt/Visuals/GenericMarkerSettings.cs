using UnityEngine;

namespace Oxide.Ext.IlovepatatosExt;

[Serializable]
public class GenericMarkerSettings
{
    public float Radius = 50;
    public float Alpha = 1;
    public string HexInnerColor = "#0f961d", HexOuterColor = "#000000";

    public Color InnerColor()
    {
        return ColorUtility.ParseHexString(HexInnerColor);
    }

    public Color OuterColor()
    {
        return ColorUtility.ParseHexString(HexOuterColor);
    }
}