﻿using System.Text;
using JetBrains.Annotations;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class StringArrayEx
{
    public static bool HasArgs(this string[] args, int index)
    {
        return args != null && args.Length > index;
    }

    public static string GetString(this string[] args, int index, string fallback = "")
    {
        return args.HasArgs(index) ? args[index] : fallback;
    }

    public static string GetAllStrings(this string[] args, int from)
    {
        StringBuilder sb = PoolUtility.Get<StringBuilder>();
        sb.Clear();

        for (var i = from; i < args.Length; i++)
            sb.Append($"{args.GetString(i)} ");

        string text = sb.ToString().Trim();
        PoolUtility.Free(ref sb);
        return text;
    }

    public static bool GetBool(this string[] args, int index, bool fallback = false)
    {
        string s = args.GetString(index);
        return bool.TryParse(s, out bool result) ? result : fallback;
    }

    public static float GetFloat(this string[] args, int index, float fallback = 0)
    {
        string s = args.GetString(index);
        return float.TryParse(s, out float result) ? result : fallback;
    }

    public static int GetInt(this string[] args, int index, int fallback = 0)
    {
        string s = args.GetString(index);
        return int.TryParse(s, out int result) ? result : fallback;
    }

    public static uint GetUint(this string[] args, int index, uint fallback = 0)
    {
        string s = args.GetString(index);
        return uint.TryParse(s, out uint result) ? result : fallback;
    }

    public static long GetLong(this string[] args, int index, long fallback = 0)
    {
        string s = args.GetString(index);
        return long.TryParse(s, out long result) ? result : fallback;
    }

    public static ulong GetUlong(this string[] args, int index, ulong fallback = 0)
    {
        string s = args.GetString(index);
        return ulong.TryParse(s, out ulong result) ? result : fallback;
    }
}