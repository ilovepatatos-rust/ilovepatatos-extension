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
        Facepunch.Pool.Free(ref obj);
    }

    public static void Free(ref StringBuilder sb)
    {
        sb.Clear();
        Facepunch.Pool.Free(ref sb);
    }

    public static void Free<T>(ref List<T> list)
    {
        list.Clear();
        Facepunch.Pool.Free(ref list);
    }

    public static void Free<T>(ref Stack<T> stack)
    {
        stack.Clear();
        Facepunch.Pool.Free(ref stack);
    }

    public static void Free<T>(ref HashSet<T> set)
    {
        set.Clear();
        Facepunch.Pool.Free(ref set);
    }

    public static void Free<TKey, TValue>(ref Dictionary<TKey, TValue> dict)
    {
        dict.Clear();
        Facepunch.Pool.Free(ref dict);
    }

    public static void Free<T>(ref ConcurrentBag<T> list)
    {
        list.Clear();
        Facepunch.Pool.Free(ref list);
    }
}