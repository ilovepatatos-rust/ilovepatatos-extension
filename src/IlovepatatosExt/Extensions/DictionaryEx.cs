using JetBrains.Annotations;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class DictionaryEx
{
    public static TValue GetOrFallback<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue fallback = default)
    {
        return key == null ? fallback : dict.GetValueOrDefault(key, fallback);
    }

    public static TValue GetOrFallbackPlusRemove<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue fallback = default)
    {
        return key == null ? fallback : dict.Remove(key, out TValue value) ? value : fallback;
    }

    /// <summary>
    /// Return whether the key was overwritten.
    /// </summary>
    public static bool ForceAdd<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue value)
    {
        if (dict.TryAdd(key, value))
            return false;

        dict[key] = value;
        return true;
    }

    public static float Increase<TKey>(this Dictionary<TKey, float> dict, TKey key, float value)
    {
        dict.TryAdd(key, default);
        dict[key] += value;

        return dict[key];
    }

    public static int Increase<TKey>(this Dictionary<TKey, int> dict, TKey key, int value)
    {
        dict.TryAdd(key, default);
        dict[key] += value;

        return dict[key];
    }

    public static long Increase<TKey>(this Dictionary<TKey, long> dict, TKey key, long value)
    {
        dict.TryAdd(key, default);
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

    /// <summary>
    /// Returns the key with the highest value.
    /// </summary>
    public static TKey GetMostPreventKey<TKey>(this Dictionary<TKey, int> self)
    {
        int highestAmount = 0;
        TKey highest = default;

        foreach ((TKey key, int amount) in self)
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
    public static TValue GetMostPresentValue<TKey, TValue>(this Dictionary<TKey, TValue> self)
    {
        var dict = PoolUtility.Get<Dictionary<TValue, int>>();

        foreach (TValue value in self.Values)
        {
            if (!dict.TryAdd(value, 1))
                dict[value]++;
        }

        int highestAmount = 0;
        TValue highest = default;

        foreach ((TValue key, int amount) in dict)
        {
            if (amount <= highestAmount)
                continue;

            highestAmount = amount;
            highest = key;
        }

        PoolUtility.Free(ref dict);
        return highest;
    }
}