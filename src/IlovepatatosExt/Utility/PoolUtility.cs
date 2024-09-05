using System.Collections.Concurrent;
using System.Text;
using Facepunch;
using JetBrains.Annotations;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class PoolUtility
{
    public static T Get<T>() where T : class, new()
    {
        return Pool.Get<T>();
    }

    public static void Free<T>(ref T obj) where T : class, new()
    {
        if (obj != null)
            Pool.FreeUnsafe(ref obj);
    }

    public static void Free(ref StringBuilder sb)
    {
        if (sb != null)
            Pool.FreeUnmanaged(ref sb);
    }

    public static void Free<T>(ref List<T> list)
    {
        if (list != null)
            Pool.FreeUnmanaged(ref list);
    }

    public static void Free<T>(ref List<T> list, bool freeElements) where T : class, Pool.IPooled, new()
    {
        if (list != null)
            Pool.Free(ref list, freeElements);
    }

    public static void Free<T>(ref Queue<T> queue)
    {
        if (queue != null)
            Pool.FreeUnmanaged(ref queue);
    }

    public static void Free<T>(ref Queue<T> queue, bool freeElements) where T : class, Pool.IPooled, new()
    {
        if (queue != null)
            Pool.Free(ref queue, freeElements);
    }

    public static void Free<T>(ref Stack<T> stack)
    {
        if (stack == null)
            return;

        stack.Clear();
        Pool.FreeUnsafe(ref stack);
    }

    public static void Free<T>(ref Stack<T> stack, bool freeElements) where T : class, Pool.IPooled, new()
    {
        if (stack == null)
            return;

        if (freeElements)
        {
            foreach (T value in stack)
            {
                T temp = value;

                if (temp != null)
                    Free(ref temp);
            }
        }

        stack.Clear();
        Pool.FreeUnsafe(ref stack);
    }

    public static void Free<T>(ref HashSet<T> set)
    {
        if (set != null)
            Pool.FreeUnmanaged(ref set);
    }

    public static void Free<T>(ref HashSet<T> set, bool freeElements) where T : class, Pool.IPooled, new()
    {
        if (set != null)
            Pool.Free(ref set, freeElements);
    }

    public static void Free<TKey, TValue>(ref Dictionary<TKey, TValue> dict)
    {
        if (dict != null)
            Pool.FreeUnmanaged(ref dict);
    }

    public static void Free<T>(ref ConcurrentBag<T> list)
    {
        if (list == null)
            return;

        list.Clear();
        Pool.FreeUnsafe(ref list);
    }

    public static void Free<T>(ref ConcurrentBag<T> list, bool freeElements) where T : class, Pool.IPooled, new()
    {
        if (list == null)
            return;

        if (freeElements)
        {
            foreach (T value in list)
            {
                T temp = value;

                if (temp != null)
                    Free(ref temp);
            }
        }

        list.Clear();
        Pool.FreeUnsafe(ref list);
    }
}