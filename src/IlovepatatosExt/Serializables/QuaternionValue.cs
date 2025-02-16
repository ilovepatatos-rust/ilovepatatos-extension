using Newtonsoft.Json;
using ProtoBuf;
using UnityEngine;

namespace Oxide.Ext.IlovepatatosExt;

[Serializable]
[ProtoContract]
public struct QuaternionValue
{
    [ProtoMember(1)]
    [JsonProperty("X")]
    public float X;

    [ProtoMember(2)]
    [JsonProperty("Y")]
    public float Y;

    [ProtoMember(3)]
    [JsonProperty("Z")]
    public float Z;

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