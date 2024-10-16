using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Oxide.Ext.IlovepatatosExt;

internal static class JsonConvertersHandler
{
    internal static void Initialize()
    {
        JsonConvert.DefaultSettings = () => new JsonSerializerSettings
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
            },
            Culture = CultureInfo.InvariantCulture
        };
    }
}