using JetBrains.Annotations;
using Newtonsoft.Json;
using Oxide.Ext.GizmosExt;
using UnityEngine;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class BoxBoundary : IBoundary
{
    [Serializable]
    public class Settings
    {
        [JsonProperty("Position")]
        public Vector3Value Pos;

        [JsonProperty("Rotation")]
        public QuaternionValue Rot;

        [JsonProperty("Size")]
        public Vector3Value Size = Vector3.one;
    }

    public Vector3 Pos;
    public Quaternion Rot;
    public Vector3 Size;

    public bool Contains(Vector3 point)
    {
        Vector3 localPoint = Quaternion.Inverse(Rot) * (point - Pos);

        return Mathf.Abs(localPoint.x) <= Size.x / 2 &&
               Mathf.Abs(localPoint.y) <= Size.y / 2 &&
               Mathf.Abs(localPoint.z) <= Size.z / 2;
    }

    public void DrawGizmos(BasePlayer player, float duration, Color color)
    {
        OxideGizmos.Box(player, Pos, Rot, Size, color, duration);
    }

    public static BoxBoundary CreateAt(Settings settings, Transform trans)
    {
        return CreateAt(settings, trans.position, trans.rotation, trans.localScale);
    }

    public static BoxBoundary CreateAt(Settings settings, Vector3 position, Quaternion rotation, Vector3 scale)
    {
        Matrix4x4 matrix = Matrix4x4.TRS(position, rotation, scale);

        Vector3 pos = matrix.MultiplyPoint3x4(settings.Pos);
        Quaternion rot = rotation * settings.Rot;

        return new BoxBoundary
        {
            Pos = pos,
            Rot = rot,
            Size = settings.Size
        };
    }
}