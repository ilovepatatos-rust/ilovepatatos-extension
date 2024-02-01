using JetBrains.Annotations;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class ObjectEx
{
    public static bool ToBool(this object obj)
    {
        if (obj is bool value)
            return value;

        return obj != null;
    }
}