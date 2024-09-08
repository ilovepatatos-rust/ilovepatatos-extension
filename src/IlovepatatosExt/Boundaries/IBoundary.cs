using JetBrains.Annotations;
using UnityEngine;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public interface IBoundary
{
    bool Contains(Vector3 pos);
    void DrawGizmos(BasePlayer player, float duration, Color color);
}