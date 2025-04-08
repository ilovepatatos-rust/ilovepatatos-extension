using System.Text;
using JetBrains.Annotations;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class StringArrayEx
{
    [MustUseReturnValue]
    public static bool HasArgs(this string[] args, int index)
    {
        return args != null && args.Length > index;
    }

    [MustUseReturnValue]
    public static string GetString(this string[] args, int index, string fallback = "")
    {
        return args.HasArgs(index) ? args[index] : fallback;
    }

    [MustUseReturnValue]
    public static string GetAllStrings(this string[] args, int from)
    {
        var sb = PoolUtility.Get<StringBuilder>();
        sb.Clear();

        for (var i = from; i < args.Length; i++)
            sb.Append($"{args.GetString(i)} ");

        string text = sb.ToString().Trim();
        PoolUtility.Free(ref sb);

        return text;
    }

    [MustUseReturnValue]
    public static bool GetBool(this string[] args, int index, bool fallback = false)
    {
        string value = args.GetString(index);

        if (string.Equals(value, "1") || string.Equals(value, "true", StringComparison.InvariantCultureIgnoreCase))
            return true;

        if (string.Equals(value, "0") || string.Equals(value, "false", StringComparison.InvariantCultureIgnoreCase))
            return false;

        return fallback;
    }

    [MustUseReturnValue]
    public static float GetFloat(this string[] args, int index, float fallback = 0)
    {
        string s = args.GetString(index);
        return float.TryParse(s, out float result) ? result : fallback;
    }

    [MustUseReturnValue]
    public static int GetInt(this string[] args, int index, int fallback = 0)
    {
        string s = args.GetString(index);
        return int.TryParse(s, out int result) ? result : fallback;
    }

    [MustUseReturnValue]
    public static uint GetUint(this string[] args, int index, uint fallback = 0)
    {
        string s = args.GetString(index);
        return uint.TryParse(s, out uint result) ? result : fallback;
    }

    [MustUseReturnValue]
    public static long GetLong(this string[] args, int index, long fallback = 0)
    {
        string s = args.GetString(index);
        return long.TryParse(s, out long result) ? result : fallback;
    }

    [MustUseReturnValue]
    public static ulong GetUlong(this string[] args, int index, ulong fallback = 0)
    {
        string s = args.GetString(index);
        return ulong.TryParse(s, out ulong result) ? result : fallback;
    }
}