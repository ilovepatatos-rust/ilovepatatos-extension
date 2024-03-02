using UnityEngine;

namespace Oxide.Ext.IlovepatatosExt;

public interface IBoundary
{
    bool Contains(Vector3 pos);
}