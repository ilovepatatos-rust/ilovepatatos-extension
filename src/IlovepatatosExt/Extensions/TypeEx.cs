using JetBrains.Annotations;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class TypeEx
{
    public static IEnumerable<Type> GetGenericTypes(this Type type)
    {
        while (type != null)
        {
            if (type.IsGenericType)
                yield return type;

            type = type.BaseType;
        }
    }
}