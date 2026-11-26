using System.Text;
using JetBrains.Annotations;
using UnityEngine;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class StringArrayEx
{
    extension(string[] args)
    {
        [MustUseReturnValue]
        public bool HasArgs(int index)
        {
            return args != null && args.Length > index;
        }

        [MustUseReturnValue]
        public string GetString(int index, string fallback = "")
        {
            return args.HasArgs(index) ? args[index] : fallback;
        }

        [MustUseReturnValue]
        public string GetAllStrings(int from)
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
        public bool GetBool(int index, bool fallback = false)
        {
            string value = args.GetString(index);

            if (string.Equals(value, "1") || string.Equals(value, "true", StringComparison.InvariantCultureIgnoreCase))
                return true;

            if (string.Equals(value, "0") || string.Equals(value, "false", StringComparison.InvariantCultureIgnoreCase))
                return false;

            return fallback;
        }

        [MustUseReturnValue]
        public float GetFloat(int index, float fallback = 0)
        {
            string s = args.GetString(index);
            return float.TryParse(s, out float result) ? result : fallback;
        }

        [MustUseReturnValue]
        public int GetInt(int index, int fallback = 0)
        {
            string s = args.GetString(index);
            return int.TryParse(s, out int result) ? result : fallback;
        }

        [MustUseReturnValue]
        public uint GetUint(int index, uint fallback = 0)
        {
            string s = args.GetString(index);
            return uint.TryParse(s, out uint result) ? result : fallback;
        }

        [MustUseReturnValue]
        public long GetLong(int index, long fallback = 0)
        {
            string s = args.GetString(index);
            return long.TryParse(s, out long result) ? result : fallback;
        }

        [MustUseReturnValue]
        public ulong GetUlong(int index, ulong fallback = 0)
        {
            string s = args.GetString(index);
            return ulong.TryParse(s, out ulong result) ? result : fallback;
        }

        [MustUseReturnValue]
        public Color GetColor(int index, Color fallback = default)
        {
            string str = GetString(args, index);
            return string.IsNullOrEmpty(str) ? fallback : str.ToColor();
        }

        [MustUseReturnValue]
        public Vector3 GetVector3(int index, Vector3 fallback = default)
        {
            string str = GetString(args, index);
            return string.IsNullOrEmpty(str) ? fallback : str.ToVector3();
        }

        [MustUseReturnValue]
        public Quaternion GetQuaternion(int index, Vector3 fallback = default)
        {
            Vector3 euler = GetVector3(args, index, fallback);
            return Quaternion.Euler(euler);
        }
    }
}