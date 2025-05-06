using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using Facepunch;
using JetBrains.Annotations;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class PoolUtility
{
    [MustUseReturnValue]
    public static T Get<T>() where T : class, new()
    {
        return Pool.Get<T>();
    }

    [MustUseReturnValue]
    public static Stopwatch GetNewStopWatch(bool start = true)
    {
        var sw = Get<Stopwatch>();

        if (start)
            sw.Restart();

        return sw;
    }

    public static void FreeNoT(ref object obj)
    {
        if (obj == null)
            return;

        Type type = obj.GetType();
        if (type.IsAbstract || type.IsInterface)
            return;

        if (type.GetConstructor(Type.EmptyTypes) == null)
            return;

        if (!Pool.Directory.TryGetValue(type, out Pool.IPoolCollection collection))
            return;

        collection.Add(obj);
        obj = null;
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

    public static void Free(ref Stopwatch sw)
    {
        if (sw == null)
            return;

        sw.Reset();
        Pool.FreeUnsafe(ref sw);
    }

    public static void Free(ref MemoryStream stream)
    {
        if (stream != null)
            Pool.FreeUnmanaged(ref stream);
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

    public static void FreeNoT<T>(ref List<T> list, bool freeElements)
    {
        if (list == null)
            return;

        if (freeElements)
        {
            foreach (T value in list)
            {
                object toFree = value;
                FreeNoT(ref toFree);
            }
        }

        Pool.FreeUnmanaged(ref list);
    }

    public static void FreeValues<T>(List<T> list) where T : class, Pool.IPooled, new()
    {
        if (list == null)
            return;

        foreach (T value in list)
        {
            T temp = value;
            Free(ref temp);
        }

        list.Clear();
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

    public static void FreeValues<T>(Queue<T> queue) where T : class, Pool.IPooled, new()
    {
        if (queue == null)
            return;

        foreach (T value in queue)
        {
            T temp = value;
            Free(ref temp);
        }

        queue.Clear();
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
            FreeValues(stack);

        stack.Clear();
        Pool.FreeUnsafe(ref stack);
    }

    public static void FreeValues<T>(Stack<T> stack) where T : class, Pool.IPooled, new()
    {
        if (stack == null)
            return;

        foreach (T value in stack)
        {
            T temp = value;
            Free(ref temp);
        }

        stack.Clear();
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

    public static void FreeValues<T>(HashSet<T> set) where T : class, Pool.IPooled, new()
    {
        if (set == null)
            return;

        foreach (T value in set)
        {
            T temp = value;
            Free(ref temp);
        }

        set.Clear();
    }

    public static void Free<TKey, TValue>(ref Dictionary<TKey, TValue> dict)
    {
        if (dict != null)
            Pool.FreeUnmanaged(ref dict);
    }

    public static void Free<TKey, TValue>(ref Dictionary<TKey, TValue> dict, bool freeElements) where TValue : class, Pool.IPooled, new()
    {
        if (dict == null)
            return;

        if (freeElements)
            FreeValues(dict);

        dict.Clear();
        Pool.FreeUnsafe(ref dict);
    }

    public static void FreeValues<TKey, TValue>(Dictionary<TKey, TValue> dict) where TValue : class, Pool.IPooled, new()
    {
        if (dict == null)
            return;

        foreach (TValue value in dict.Values)
        {
            TValue temp = value;
            Free(ref temp);
        }

        dict.Clear();
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
            FreeValues(list);

        list.Clear();
        Pool.FreeUnsafe(ref list);
    }

    public static void FreeValues<T>(ConcurrentBag<T> list) where T : class, Pool.IPooled, new()
    {
        if (list == null)
            return;

        foreach (T value in list)
        {
            T temp = value;
            Free(ref temp);
        }

        list.Clear();
    }

    /// <summary>
    /// Certain types get lost when reloading plugins.
    /// Use this before unloading a plugin to avoid memory leaks.
    /// </summary>
    public static void Drop(Type type)
    {
        Pool.Directory.Remove(type, out Pool.IPoolCollection _);
        DropGenericTypeWith(type);
    }

    public static void Drop(Assembly assembly)
    {
        foreach (Type type in assembly.GetTypes())
        {
            if (typeof(Pool.IPooled).IsAssignableFrom(type))
                Drop(type);
        }
    }

    private static void DropGenericTypeWith(Type targetType)
    {
        var toRemove = Get<List<Type>>();

        foreach ((Type type, Pool.IPoolCollection _) in Pool.Directory)
        {
            foreach (Type subType in type.GetGenericTypes())
            {
                if (!subType.HasAnyArgumentsOfType(targetType))
                    continue;

                toRemove.Add(type);
                break;
            }
        }

        foreach (Type type in toRemove)
            Pool.Directory.Remove(type, out Pool.IPoolCollection _);

        Free(ref toRemove);
    }
}