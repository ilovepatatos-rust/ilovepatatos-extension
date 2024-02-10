using JetBrains.Annotations;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class BoxColliderEx
{
    public static bool Contains(this BoxCollider collider, Vector3 point)
    {
        Vector3 size = collider.size;
        Vector3 localPos = collider.transform.InverseTransformPoint(point) - collider.center;

        return Mathf.Abs(localPos.x) < size.x / 2 && Mathf.Abs(localPos.y) < size.y / 2 && Mathf.Abs(localPos.z) < size.z / 2;
    }

    public static Vector3 GetRandomPointWithinBounds(this BoxCollider col, Vector3 offset = default)
    {
        Vector3 center = col.center;
        Vector3 size = col.size * 0.5f;

        float x = Random.Range(-size.x + offset.x, size.x - offset.x);
        float y = Random.Range(-size.y + offset.y, size.y - offset.y);
        float z = Random.Range(-size.z + offset.z, size.z - offset.z);

        return col.transform.TransformPoint(center + new Vector3(x, y, z));
    }
}