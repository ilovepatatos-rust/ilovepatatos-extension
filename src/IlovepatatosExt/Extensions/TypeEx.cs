using JetBrains.Annotations;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
internal static class TypeEx
{
    internal static IEnumerable<Type> GetGenericTypes(this Type type)
    {
        while (type != null)
        {
            if (type.IsGenericType)
                yield return type;

            type = type.BaseType;
        }
    }

    // recursive
    internal static bool HasAnyArgumentsOfType(this Type type, Type targetType)
    {
        foreach (Type argument in type.GetGenericArguments())
        {
            if (argument == targetType)
                return true;

            if (argument.GetGenericTypes().Any(subType => subType.HasAnyArgumentsOfType(targetType)))
                return true;
        }

        return false;
    }
}