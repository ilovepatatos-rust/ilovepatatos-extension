using JetBrains.Annotations;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class ListEx
{
    public static bool ContainsIndex<T>(this IList<T> list, int index)
    {
        return index >= 0 && list != null && index < list.Count;
    }

    public static T GetOrFallback<T>(this IList<T> list, int index, T fallback = default)
    {
        return list.ContainsIndex(index) ? list[index] : fallback;
    }

    public static T GetAtPlusRemove<T>(this IList<T> list, int index)
    {
        if (!list.ContainsIndex(index))
            throw new ArgumentOutOfRangeException();

        T item = list[index];
        list.RemoveAt(index);
        return item;
    }

    public static void Randomize<T>(this IList<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, list.Count);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}