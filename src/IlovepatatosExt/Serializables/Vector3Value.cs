using Newtonsoft.Json;
using ProtoBuf;
using UnityEngine;

namespace Oxide.Ext.IlovepatatosExt;

[Serializable]
[ProtoContract]
public struct Vector3Value
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

    public static implicit operator Vector3Value(Vector3 value)
    {
        return new Vector3Value { X = value.x, Y = value.y, Z = value.z };
    }

    public static implicit operator Vector3(Vector3Value value)
    {
        return new Vector3(value.X, value.Y, value.Z);
    }
}