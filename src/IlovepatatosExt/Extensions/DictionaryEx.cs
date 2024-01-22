using JetBrains.Annotations;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class DictionaryEx
{
    public static TValue GetOrFallback<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue fallback = default)
    {
        if (key == null)
            return fallback;

        return dict.TryGetValue(key, out TValue value) ? value : fallback;
    }

    public static TValue GetOrFallbackPlusRemove<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue fallback = default)
    {
        if (key == null)
            return fallback;

        if (!dict.ContainsKey(key))
            return fallback;

        TValue value = dict[key];
        dict.Remove(key);
        return value;
    }

    /// <summary>
    /// Return whether or not the key was overwritten.
    /// </summary>
    public static bool ForceAdd<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue value)
    {
        if (dict.TryAdd(key, value))
            return false;

        dict[key] = value;
        return true;
    }

    /// <summary>
    /// Returns the value that is present the most in the dictionary.
    /// </summary>
    public static TValue GetMostPresentValue<TKey, TValue>(this Dictionary<TKey, TValue> self)
    {
        var dict = PoolUtility.Get<Dictionary<TValue, int>>();
        dict.Clear();

        foreach (TValue value in self.Values)
        {
            if (!dict.ContainsKey(value))
                dict.Add(value, 0);

            dict[value]++;
        }

        int highestAmount = 0;
        TValue highest = default;

        foreach (var valueToAmount in dict)
        {
            int amount = valueToAmount.Value;

            if (amount <= highestAmount)
                continue;

            highestAmount = amount;
            highest = valueToAmount.Key;
        }

        PoolUtility.Free(ref dict);
        return highest;
    }
}