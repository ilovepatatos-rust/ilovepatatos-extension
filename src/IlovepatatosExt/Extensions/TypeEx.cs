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

    internal static bool HasAnyArgumentsOfType(this Type type, Type targetType)
    {
        var visited = PoolUtility.Get<HashSet<Type>>();
        bool result = HasAnyArgumentsOfType(type, targetType, visited);

        PoolUtility.Free(ref visited);
        return result;
    }

    // recursive
    private static bool HasAnyArgumentsOfType(Type type, Type targetType, HashSet<Type> visited)
    {
        if (!visited.Add(type))
            return false;

        foreach (Type argument in type.GetGenericArguments())
        {
            if (argument == targetType)
                return true;

            foreach (Type genericType in argument.GetGenericTypes())
            {
                if (HasAnyArgumentsOfType(genericType, targetType, visited))
                    return true;
            }
        }

        return false;
    }
}