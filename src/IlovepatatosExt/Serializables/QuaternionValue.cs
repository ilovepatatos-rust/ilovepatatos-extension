using UnityEngine;

namespace Oxide.Ext.IlovepatatosExt;

[Serializable]
public struct QuaternionValue
{
    public float X, Y, Z;

    public static implicit operator QuaternionValue(Quaternion value)
    {
        Vector3 rot = value.eulerAngles;
        return new QuaternionValue { X = rot.x, Y = rot.y, Z = rot.z };
    }

    public static implicit operator Quaternion(QuaternionValue value)
    {
        return Quaternion.Euler(value.X, value.Y, value.Z);
    }
}