using UnityEngine;

namespace Oxide.Ext.IlovepatatosExt;

[Serializable]
public struct Vector3Value
{
    public float X, Y, Z;

    public static implicit operator Vector3Value(Vector3 value)
    {
        return new Vector3Value { X = value.x, Y = value.y, Z = value.z };
    }

    public static implicit operator Vector3(Vector3Value value)
    {
        return new Vector3(value.X, value.Y, value.Z);
    }
}