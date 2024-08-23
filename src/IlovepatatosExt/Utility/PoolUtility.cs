using System.Collections.Concurrent;
using System.Text;
using JetBrains.Annotations;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class PoolUtility
{
    public static T Get<T>() where T : class, new()
    {
        return Facepunch.Pool.Get<T>();
    }

    public static void Free<T>(ref T obj) where T : class, new()
    {
        FreeInternal(ref obj);
    }

    public static void Free(ref StringBuilder sb)
    {
        sb?.Clear();
        FreeInternal(ref sb);
    }

    public static void Free<T>(ref List<T> list)
    {
        list?.Clear();
        FreeInternal(ref list);
    }

    public static void Free<T>(ref Stack<T> stack)
    {
        stack?.Clear();
        FreeInternal(ref stack);
    }

    public static void Free<T>(ref HashSet<T> set)
    {
        set?.Clear();
        FreeInternal(ref set);
    }

    public static void Free<TKey, TValue>(ref Dictionary<TKey, TValue> dict)
    {
        dict?.Clear();
        FreeInternal(ref dict);
    }

    public static void Free<T>(ref ConcurrentBag<T> list)
    {
        list?.Clear();
        FreeInternal(ref list);
    }

    public static void FreeInternal<T>(ref T obj) where T : class, new()
    {
        if (obj != null)
            Facepunch.Pool.Free(ref obj);
    }
}