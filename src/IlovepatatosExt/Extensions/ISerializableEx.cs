using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Oxide.Ext.IlovepatatosExt;

// ReSharper disable once InconsistentNaming
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class ISerializableEx
{
    extension<T>(T obj) where T : ISerializable
    {
        [MustUseReturnValue]
        public T Clone()
        {
            string json = JsonConvert.SerializeObject(obj);
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}