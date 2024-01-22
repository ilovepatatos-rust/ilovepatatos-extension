using JetBrains.Annotations;

namespace Oxide.Ext.IlovepatatosExt;

// ReSharper disable once InconsistentNaming
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class IEnumerableEx
{
    public static List<T> ToPooledList<T>(this IEnumerable<T> enumerable)
    {
        var list = PoolUtility.Get<List<T>>();
        list.AddRange(enumerable);
        return list;
    }

    public static T PickRandom<T>(this IEnumerable<T> enumerable)
    {
        if (enumerable is List<T> value)
            return value.GetRandom();

        var list = enumerable.ToPooledList();
        T result = list.GetRandom();
        PoolUtility.Free(ref list);

        return result;
    }
}