using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Oxide.Ext.IlovepatatosExt;

// ReSharper disable once InconsistentNaming
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class ISerializableEx
{
    [MustUseReturnValue]
    public static T Clone<T>(this T obj) where T : ISerializable
    {
        string json = JsonConvert.SerializeObject(obj);
        return JsonConvert.DeserializeObject<T>(json);
    }
}