using JetBrains.Annotations;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class BoxColliderEx
{
    extension(BoxCollider collider)
    {
        [MustUseReturnValue]
        public bool Contains(Vector3 point)
        {
            Vector3 size = collider.size;
            Vector3 localPos = collider.transform.InverseTransformPoint(point) - collider.center;

            return Mathf.Abs(localPos.x) < size.x / 2 && Mathf.Abs(localPos.y) < size.y / 2 && Mathf.Abs(localPos.z) < size.z / 2;
        }

        [MustUseReturnValue]
        public Vector3 GetRandomPointWithinBounds(Vector3 offset = default)
        {
            Vector3 center = collider.center;
            Vector3 size = collider.size * 0.5f;

            float x = Random.Range(-size.x + offset.x, size.x - offset.x);
            float y = Random.Range(-size.y + offset.y, size.y - offset.y);
            float z = Random.Range(-size.z + offset.z, size.z - offset.z);

            return collider.transform.TransformPoint(center + new Vector3(x, y, z));
        }
    }
}