using JetBrains.Annotations;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class ObjectEx
{
    extension(object obj)
    {
        [MustUseReturnValue]
        public T Cast<T>(T fallback = default)
        {
            if (obj is T value)
                return value;

            return fallback;
        }

        [MustUseReturnValue]
        public bool ToBool()
        {
            if (obj is bool value)
                return value;

            return obj != null;
        }
    }
}