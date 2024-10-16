using Newtonsoft.Json;
using Newtonsoft.Json.UnityConverters;
using UnityEngine;

namespace Oxide.Ext.IlovepatatosExt.Newtonsoft;

/// <summary>
/// Custom Newtonsoft.Json converter <see cref="JsonConverter"/> for the Unity Matrix4x4 type <see cref="Matrix4x4"/>.
/// </summary>
// ReSharper disable once InconsistentNaming
public class Matrix4x4Converter : PartialConverter<Matrix4x4>
{
    // https://github.com/Unity-Technologies/UnityCsReference/blob/2019.2/Runtime/Export/Math/Matrix4x4.cs#L21-L29
    private static readonly string[] _names = GetMemberNames();
    private static readonly Dictionary<string, int> _namesToIndex = GetNamesToIndex(_names);

    /// <summary>
    /// Get the property names include from <c>m00</c> to <c>m33</c>.
    /// </summary>
    /// <returns>The property names.</returns>
    private static string[] GetMemberNames()
    {
        string[] indexes = new[] { "0", "1", "2", "3" };
        return indexes.SelectMany((row) => indexes.Select((column) => "m" + column + row)).ToArray();
    }

    // Reusing the same strings here instead of creating new ones. Tiny bit lower memory footprint
    private static Dictionary<string, int> GetNamesToIndex(string[] names)
    {
        var dict = new Dictionary<string, int>();
        for (int i = 0; i < names.Length; i++)
        {
            dict[names[i]] = i;
        }

        return dict;
    }

    protected override void ReadValue(ref Matrix4x4 value, string name, JsonReader reader, JsonSerializer serializer)
    {
        if (_namesToIndex.TryGetValue(name, out var index))
        {
            value[index] = reader.ReadAsFloat() ?? 0;
        }
    }

    protected override void WriteJsonProperties(JsonWriter writer, Matrix4x4 value, JsonSerializer serializer)
    {
        for (int i = 0; i < _names.Length; i++)
        {
            writer.WritePropertyName(_names[i]);
            writer.WriteValue(value[i]);
        }
    }
}