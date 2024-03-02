using JetBrains.Annotations;
using Newtonsoft.Json;
using Oxide.Ext.GizmosExt;
using UnityEngine;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class SphereBoundary : IBoundary
{
    [Serializable]
    public class Settings
    {
        [JsonProperty("Position")]
        public Vector3Value Pos;

        [JsonProperty("Radius")]
        public float Radius;
    }

    public Vector3 Pos;
    public float Radius;

    public bool Contains(Vector3 pos)
    {
        return (Pos - pos).sqrMagnitude <= Radius * Radius;
    }

    public void DrawGizmos(BasePlayer player, float duration, Color color)
    {
        OxideGizmos.Sphere(player, Pos, Radius, color, duration);
    }

    [UsedImplicitly]
    public static SphereBoundary CreateAt(Settings settings, Transform trans)
    {
        return CreateAt(settings, trans.position, trans.rotation, trans.localScale);
    }

    [UsedImplicitly]
    public static SphereBoundary CreateAt(Settings settings, Vector3 position, Quaternion rotation, Vector3 scale)
    {
        Matrix4x4 matrix = Matrix4x4.TRS(position, rotation, scale);
        Vector3 pos = matrix.MultiplyPoint3x4(settings.Pos);

        return new SphereBoundary
        {
            Pos = pos,
            Radius = settings.Radius
        };
    }
}