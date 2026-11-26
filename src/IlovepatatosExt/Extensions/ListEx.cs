using JetBrains.Annotations;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class ListEx
{
    extension<T>(IList<T> list)
    {
        [MustUseReturnValue]
        public bool ContainsIndex(int index)
        {
            return index >= 0 && list != null && index < list.Count;
        }

        [MustUseReturnValue]
        public T GetOrFallback(int index, T fallback = default)
        {
            return list.ContainsIndex(index) ? list[index] : fallback;
        }

        [MustUseReturnValue]
        public T GetAtPlusRemove(int index)
        {
            if (!list.ContainsIndex(index))
                throw new ArgumentOutOfRangeException();

            T item = list[index];
            list.RemoveAt(index);
            return item;
        }

        public void Randomize()
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = UnityEngine.Random.Range(0, list.Count);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }
    }
}