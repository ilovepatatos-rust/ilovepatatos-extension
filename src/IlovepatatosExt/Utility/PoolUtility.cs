using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Facepunch;
using JetBrains.Annotations;
using Oxide.Core;
using Oxide.Ext.ConsoleExt;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class PoolUtility
{
    public static bool TrackCapacity;
    public static int CapacityThreshold = 1024;

    private static readonly string s_capacityFilename = $"capacity_{DateTime.Now:yyyy-MM-dd_HH-mm}.txt";

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
        if (list == null)
            return;

        list.Clear();

        if (TrackCapacity)
        {
            int capacity = list.Capacity;
            if (IsOverThreshold(capacity))
            {
                MarkCapacityOverThreshold<T>(list.GetType(), capacity);
                list.TrimExcess(); // reset to default capacity
            }
        }

        Pool.FreeUnsafe(ref list);
    }

    public static void Free<T>(ref List<T> list, bool freeElements) where T : class, new()
    {
        if (list == null)
            return;

        if (freeElements)
            FreeValues(list);

        Free(ref list);
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

    public static void FreeValues<T>(List<T> list) where T : class, new()
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
        if (queue == null)
            return;

        queue.Clear();

        if (TrackCapacity)
        {
            int capacity = queue.Capacity();
            if (IsOverThreshold(capacity))
            {
                MarkCapacityOverThreshold<T>(queue.GetType(), capacity);
                queue.TrimExcess(); // reset to default capacity
            }
        }

        Pool.FreeUnsafe(ref queue);
    }

    public static void Free<T>(ref Queue<T> queue, bool freeElements) where T : class, new()
    {
        if (queue == null)
            return;

        if (freeElements)
            FreeValues(queue);

        Free(ref queue);
    }

    public static void FreeValues<T>(Queue<T> queue) where T : class, new()
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

        if (TrackCapacity)
        {
            int capacity = stack.Capacity();
            if (IsOverThreshold(capacity))
            {
                MarkCapacityOverThreshold<T>(stack.GetType(), capacity);
                stack.TrimExcess(); // reset to default capacity
            }
        }

        Pool.FreeUnsafe(ref stack);
    }

    public static void Free<T>(ref Stack<T> stack, bool freeElements) where T : class, new()
    {
        if (stack == null)
            return;

        if (freeElements)
            FreeValues(stack);

        Free(ref stack);
    }

    public static void FreeValues<T>(Stack<T> stack) where T : class, new()
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
        if (set == null)
            return;

        set.Clear();

        if (TrackCapacity)
        {
            int capacity = set.Count;
            if (IsOverThreshold(capacity))
            {
                MarkCapacityOverThreshold<T>(set.GetType(), capacity);
                set.TrimExcess(); // reset to default capacity
            }
        }

        Pool.FreeUnsafe(ref set);
    }

    public static void Free<T>(ref HashSet<T> set, bool freeElements) where T : class, new()
    {
        if (set == null)
            return;

        if (freeElements)
            FreeValues(set);

        Free(ref set);
    }

    public static void FreeValues<T>(HashSet<T> set) where T : class, new()
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
        if (dict == null)
            return;

        dict.Clear();

        if (TrackCapacity)
        {
            int capacity = dict.Capacity();
            if (IsOverThreshold(capacity))
            {
                MarkCapacityOverThreshold<TValue>(dict.GetType(), capacity);
                dict.TrimExcess(); // reset to default capacity
            }
        }

        Pool.FreeUnsafe(ref dict);
    }

    public static void Free<TKey, TValue>(ref Dictionary<TKey, TValue> dict, bool freeElements) where TValue : class, new()
    {
        if (dict == null)
            return;

        if (freeElements)
            FreeValues(dict);

        Free(ref dict);
    }

    public static void FreeValues<TKey, TValue>(Dictionary<TKey, TValue> dict) where TValue : class, new()
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

    public static void Free<T>(ref ConcurrentBag<T> list, bool freeElements) where T : class, new()
    {
        if (list == null)
            return;

        if (freeElements)
            FreeValues(list);

        list.Clear();
        Pool.FreeUnsafe(ref list);
    }

    public static void FreeValues<T>(ConcurrentBag<T> list) where T : class, new()
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

    public static void Free<T>(ref ListHashSet<T> list)
    {
        if (list == null)
            return;

        list.Clear();
        Pool.FreeUnsafe(ref list);
    }

    public static void Free<T>(ref ListHashSet<T> list, bool freeElements) where T : class, new()
    {
        if (list == null)
            return;

        if (freeElements)
            FreeValues(list);

        Free(ref list);
    }

    public static void FreeValues<T>(ListHashSet<T> list) where T : class, new()
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
            foreach (Type genericType in type.GetGenericTypes())
            {
                if (!genericType.HasAnyArgumentsOfType(targetType))
                    continue;

                toRemove.Add(type);
                break;
            }
        }

        foreach (Type type in toRemove)
            Pool.Directory.Remove(type, out Pool.IPoolCollection _);

        Free(ref toRemove);
    }

    private static bool IsOverThreshold(int capacity)
    {
        return capacity > CapacityThreshold;
    }


    private static void MarkCapacityOverThreshold<T>(Type type, int capacity)
    {
        int size = typeof(T).IsValueType ? Marshal.SizeOf<T>() : IntPtr.Size;
        OxideConsole.LogFormat("[Pool] {0} * {1} = {2} -> {3}", OxideConsole.YELLOW, capacity, size, capacity * size, type);

        string dir = Path.Combine(Interface.Oxide.LogDirectory, "Pool");
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        var sb = Get<StringBuilder>();
        sb.AppendFormat("[Pool] {0} * {1} = {2} -> {3}", capacity, size, capacity * size, type);
        sb.AppendLine();

        sb.AppendLine(new StackTrace().ToString());
        sb.AppendLine();

        string path = Path.Combine(dir, s_capacityFilename);
        using (var stream = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.Read))
        {
            using (var writer = new StreamWriter(stream, Encoding.UTF8))
                writer.WriteLine(sb.ToString());
        }

        Free(ref sb);
    }
}