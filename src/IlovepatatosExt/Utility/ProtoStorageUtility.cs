using JetBrains.Annotations;
using Oxide.Core;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class ProtoStorageUtility
{
    public static T TryLoad<T>(params string[] subPaths)
    {
        return ProtoStorage.Exists(subPaths) ? ProtoStorage.Load<T>(subPaths) : default;
    }
}