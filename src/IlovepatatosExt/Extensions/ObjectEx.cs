using JetBrains.Annotations;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class ObjectEx
{
    public static T Cast<T>(this object obj, T fallback = default)
    {
        if (obj is T value)
            return value;

        return fallback;
    }

    public static bool ToBool(this object obj)
    {
        if (obj is bool value)
            return value;

        return obj != null;
    }
}