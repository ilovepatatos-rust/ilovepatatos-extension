using JetBrains.Annotations;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class SphereColliderEx
{
    extension(SphereCollider col)
    {
        [MustUseReturnValue]
        public Vector3 GetRandomPointWithinBounds(float offset = 0)
        {
            return col.center + Random.insideUnitSphere * (col.radius - offset);
        }
    }
}