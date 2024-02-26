using Newtonsoft.Json;
using UnityEngine;

namespace Oxide.Ext.IlovepatatosExt;

[Serializable]
public struct TransformValue
{
    [SerializeField, JsonProperty("Position")]
    private Vector3 m_InternalPosition;

    [SerializeField, JsonProperty("Euler")]
    private Vector3 m_InternalRotation;

    [JsonIgnore]
    public Vector3 Pos
    {
        get => m_InternalPosition;
        set => m_InternalPosition = value;
    }

    [JsonIgnore]
    public Quaternion Rot
    {
        get => Quaternion.Euler(m_InternalRotation);
        set => m_InternalRotation = value.eulerAngles;
    }

    // ReSharper disable once InconsistentNaming
    public Matrix4x4 ToMatrix4x4()
    {
        return ToMatrix4x4(Vector3.one);
    }

    // ReSharper disable once InconsistentNaming
    public Matrix4x4 ToMatrix4x4(Vector3 size)
    {
        return Matrix4x4.TRS(Pos, Rot, size);
    }

    public static TransformValue operator *(TransformValue self, TransformValue other)
    {
        var matrix = self.ToMatrix4x4();

        Vector3 pos = matrix.MultiplyPoint3x4(other.Pos);
        Quaternion rot = self.Rot * other.Rot;

        return new TransformValue { Pos = pos, Rot = rot };
    }
}