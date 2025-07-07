using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class DictionaryEx
{
    [MustUseReturnValue]
    public static int Capacity<TKey, TValue>(this Dictionary<TKey, TValue> dict)
    {
        FieldInfo field = AccessTools.Field(dict.GetType(), "_buckets");
        int[] array = (int[])field.GetValue(dict);

        return array?.Length ?? 0;
    }

    [MustUseReturnValue]
    public static TValue GetOrFallback<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue fallback = default)
    {
        return key == null ? fallback : dict.GetValueOrDefault(key, fallback);
    }

    [MustUseReturnValue]
    public static TValue GetOrFallbackPlusRemove<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue fallback = default)
    {
        return key == null ? fallback : dict.Remove(key, out TValue value) ? value : fallback;
    }

    /// <summary>
    /// Return whether the key was overwritten.
    /// </summary>
    [Obsolete("Simply use dict[key] = value; now")]
    public static bool ForceAdd<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue value)
    {
        if (dict.TryAdd(key, value))
            return false;

        dict[key] = value;
        return true;
    }

    public static float Increase<TKey>(this Dictionary<TKey, float> dict, TKey key, float value)
    {
        if (!dict.TryAdd(key, value))
            dict[key] += value;

        return dict[key];
    }

    public static int Increase<TKey>(this Dictionary<TKey, int> dict, TKey key, int value)
    {
        if (!dict.TryAdd(key, value))
            dict[key] += value;

        return dict[key];
    }

    public static long Increase<TKey>(this Dictionary<TKey, long> dict, TKey key, long value)
    {
        if (!dict.TryAdd(key, value))
            dict[key] += value;

        return dict[key];
    }

    /// <summary>
    /// Removes all elements that satisfy the predicate.
    /// </summary>
    /// <returns>The amount of elements removed.</returns>
    public static int RemoveAll<TKey, TValue>(this Dictionary<TKey, TValue> dict, Func<TKey, bool> predicate)
    {
        var toRemove = PoolUtility.Get<HashSet<TKey>>();

        foreach (KeyValuePair<TKey, TValue> pair in dict.Where(pair => predicate(pair.Key)))
            toRemove.Add(pair.Key);

        foreach (TKey key in toRemove)
            dict.Remove(key);

        int count = toRemove.Count;

        PoolUtility.Free(ref toRemove);
        return count;
    }

    /// <summary>
    /// Removes all elements that satisfy the predicate.
    /// </summary>
    /// <returns>The amount of elements removed.</returns>
    public static int RemoveAll<TKey, TValue>(this Dictionary<TKey, TValue> dict, Func<TValue, bool> predicate)
    {
        var toRemove = PoolUtility.Get<HashSet<TKey>>();

        foreach (KeyValuePair<TKey, TValue> pair in dict.Where(pair => predicate(pair.Value)))
            toRemove.Add(pair.Key);

        foreach (TKey key in toRemove)
            dict.Remove(key);

        int count = toRemove.Count;

        PoolUtility.Free(ref toRemove);
        return count;
    }

    /// <summary>
    /// Removes all elements that satisfy the predicate.
    /// </summary>
    /// <returns>The amount of elements removed.</returns>
    public static int RemoveAll<TKey, TValue>(this Dictionary<TKey, TValue> dict, Func<TKey, TValue, bool> predicate)
    {
        var toRemove = PoolUtility.Get<HashSet<TKey>>();

        foreach (KeyValuePair<TKey, TValue> pair in dict.Where(pair => predicate(pair.Key, pair.Value)))
            toRemove.Add(pair.Key);

        foreach (TKey key in toRemove)
            dict.Remove(key);

        int count = toRemove.Count;

        PoolUtility.Free(ref toRemove);
        return count;
    }

    public static bool TryGetKey<TKey, TValue>(this Dictionary<TKey, TValue> dict, TValue value, out TKey key)
    {
        foreach ((TKey x, TValue y) in dict)
        {
            if (!EqualityComparer<TValue>.Default.Equals(y, value))
                continue;

            key = x;
            return true;
        }

        key = default;
        return false;
    }

    /// <summary>
    /// Returns the key with the highest value.
    /// </summary>
    [MustUseReturnValue]
    public static TKey GetMostPreventKey<TKey>(this Dictionary<TKey, int> dict)
    {
        int highestAmount = 0;
        TKey highest = default;

        foreach ((TKey key, int amount) in dict)
        {
            if (highest != null && amount <= highestAmount)
                continue;

            highestAmount = amount;
            highest = key;
        }

        return highest;
    }

    /// <summary>
    /// Returns the value that is present the most in the dictionary.
    /// </summary>
    [MustUseReturnValue]
    public static TValue GetMostPresentValue<TKey, TValue>(this Dictionary<TKey, TValue> dict)
    {
        var valueToCount = PoolUtility.Get<Dictionary<TValue, int>>();

        foreach (TValue value in dict.Values)
        {
            if (!valueToCount.TryAdd(value, 1))
                valueToCount[value]++;
        }

        int highestAmount = 0;
        TValue highest = default;

        foreach ((TValue key, int amount) in valueToCount)
        {
            if (amount <= highestAmount)
                continue;

            highestAmount = amount;
            highest = key;
        }

        PoolUtility.Free(ref valueToCount);
        return highest;
    }
}