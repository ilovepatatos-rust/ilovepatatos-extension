using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class DictionaryEx
{
    extension<TKey, TValue>(Dictionary<TKey, TValue> dict)
    {
        [MustUseReturnValue]
        public int Capacity()
        {
            FieldInfo field = AccessTools.Field(dict.GetType(), "_buckets");
            int[] array = (int[])field.GetValue(dict);

            return array?.Length ?? 0;
        }

        [MustUseReturnValue]
        public TValue GetOrFallback(TKey key, TValue fallback = default)
        {
            return key == null ? fallback : dict.GetValueOrDefault(key, fallback);
        }

        [MustUseReturnValue]
        public TValue GetOrFallbackPlusRemove(TKey key, TValue fallback = default)
        {
            return key == null ? fallback : dict.Remove(key, out TValue value) ? value : fallback;
        }

        /// <summary>
        /// Return whether the key was overwritten.
        /// </summary>
        [Obsolete("Simply use dict[key] = value; now")]
        public bool ForceAdd(TKey key, TValue value)
        {
            if (dict.TryAdd(key, value))
                return false;

            dict[key] = value;
            return true;
        }

        /// <summary>
        /// Removes all elements that satisfy the predicate.
        /// </summary>
        /// <returns>The amount of elements removed.</returns>
        public int RemoveAll(Func<TKey, bool> predicate)
        {
            var toRemove = PoolUtility.Get<HashSet<TKey>>();

            foreach (TKey key in dict.Keys)
            {
                if (predicate(key))
                    toRemove.Add(key);
            }

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
        public int RemoveAll(Func<TValue, bool> predicate)
        {
            var toRemove = PoolUtility.Get<HashSet<TKey>>();

            foreach (KeyValuePair<TKey, TValue> keyToValue in dict)
            {
                if (predicate(keyToValue.Value))
                    toRemove.Add(keyToValue.Key);
            }

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
        public int RemoveAll(Func<TKey, TValue, bool> predicate)
        {
            var toRemove = PoolUtility.Get<HashSet<TKey>>();

            foreach (KeyValuePair<TKey, TValue> keyToValue in dict)
            {
                if (predicate(keyToValue.Key, keyToValue.Value))
                    toRemove.Add(keyToValue.Key);
            }

            foreach (TKey key in toRemove)
                dict.Remove(key);

            int count = toRemove.Count;

            PoolUtility.Free(ref toRemove);
            return count;
        }

        public bool TryGetKey(TValue value, out TKey key)
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
        /// Returns the value that is present the most in the dictionary.
        /// </summary>
        [MustUseReturnValue]
        public TValue GetMostPresentValue()
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

    extension<TKey>(Dictionary<TKey, float> dict)
    {
        public float Increase(TKey key, float value)
        {
            if (!dict.TryAdd(key, value))
                dict[key] += value;

            return dict[key];
        }
    }

    extension<TKey>(Dictionary<TKey, int> dict)
    {
        public int Increase(TKey key, int value)
        {
            if (!dict.TryAdd(key, value))
                dict[key] += value;

            return dict[key];
        }

        /// <summary>
        /// Returns the key with the highest value.
        /// </summary>
        [MustUseReturnValue]
        public TKey GetMostPreventKey()
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
    }

    extension<TKey>(Dictionary<TKey, long> dict)
    {
        public long Increase(TKey key, long value)
        {
            if (!dict.TryAdd(key, value))
                dict[key] += value;

            return dict[key];
        }
    }
}