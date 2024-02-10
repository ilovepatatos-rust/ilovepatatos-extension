using JetBrains.Annotations;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
internal static class SphereColliderEx
{
    internal static Vector3 GetRandomPointWithinBounds(this SphereCollider col, float offset = 0)
    {
        return col.center + Random.insideUnitSphere * (col.radius - offset);
    }
}