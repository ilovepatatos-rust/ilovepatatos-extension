using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Oxide.Game.Rust.Cui;
using UnityEngine;

namespace Oxide.Ext.IlovepatatosExt;

internal static class JsonConvertersHandler
{
    private static readonly JsonSerializerSettings s_settings = new()
    {
        Converters = new List<JsonConverter>
        {
            new Newtonsoft.Color32Converter(),
            new Newtonsoft.ColorConverter(),
            new Newtonsoft.Matrix4x4Converter(),
            new Newtonsoft.QuaternionConverter(),
            new Newtonsoft.Vector2Converter(),
            new Newtonsoft.Vector2IntConverter(),
            new Newtonsoft.Vector3Converter(),
            new Newtonsoft.Vector3IntConverter(),
            new Newtonsoft.Vector4Converter(),
            new HashSetConverter(),
            new KeyValuePairConverter(),
            new RegexConverter(),
            new StringEnumConverter(),
            new ComponentConverter()
        },
        Culture = CultureInfo.InvariantCulture
    };

    internal static void Initialize()
    {
        JsonConvert.DefaultSettings = () => s_settings;
    }
}