using JetBrains.Annotations;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class ArrayEx
{
    public static void Randomize<T>(this T[] array)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, array.Length);
            (array[i], array[j]) = (array[j], array[i]);
        }
    }
}