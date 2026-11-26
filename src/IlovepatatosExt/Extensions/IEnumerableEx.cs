using JetBrains.Annotations;

namespace Oxide.Ext.IlovepatatosExt;

// ReSharper disable once InconsistentNaming
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class IEnumerableEx
{
    extension<T>(IEnumerable<T> enumerable)
    {
        [MustUseReturnValue]
        public List<T> ToPooledList()
        {
            var list = PoolUtility.Get<List<T>>();
            list.AddRange(enumerable);
            return list;
        }

        [MustUseReturnValue]
        public T PickRandom()
        {
            if (enumerable is List<T> value)
                return value.GetRandom();

            var list = enumerable.ToPooledList();
            T result = list.GetRandom();
            PoolUtility.Free(ref list);

            return result;
        }
    }
}