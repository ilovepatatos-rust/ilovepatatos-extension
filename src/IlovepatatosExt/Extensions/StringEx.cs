using System.Text;
using JetBrains.Annotations;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class StringEx
{
    [MustUseReturnValue]
    public static string FormatNoThrow(this string format, params object[] args)
    {
        if (format == null)
            throw new ArgumentNullException(nameof(format));

        try
        {
            return string.Format(format, args);
        }
        catch
        {
            return ManuallyFormatArgs(format, args);
        }
    }

    private static string ManuallyFormatArgs(ReadOnlySpan<char> span, object[] args)
    {
        var sb = PoolUtility.Get<StringBuilder>();
        int length = span.Length;

        for (int i = 0; i < length; i++)
        {
            char c = span[i];
            int start = i + 1;

            switch (c)
            {
                case '{' when start < length && span[start] == '{':
                    sb.Append('{');
                    i++; // Skip second '{'
                    continue;
                case '{':
                {
                    int end = start;
                    while (end < length && span[end] != '}')
                        end++;

                    if (end >= length)
                    {
                        // Unclosed '{', treat as literal
                        sb.Append('{');
                        continue;
                    }

                    ReadOnlySpan<char> indexString = span.Slice(start, end - start);
                    if (int.TryParse(indexString, out int index) && index >= 0 && index < args.Length)
                    {
                        object arg = args[index];
                        sb.Append(arg?.ToString() ?? string.Empty);
                    }
                    else
                    {
                        // Leave invalid or missing tokens unchanged
                        sb.Append('{').Append(indexString).Append('}');
                    }

                    i = end; // Move past '}'
                    continue;
                }
                case '}' when start < length && span[start] == '}':
                    sb.Append('}');
                    i++; // Skip second '}'
                    continue;
                default:
                    sb.Append(c);
                    break;
            }
        }

        string text = sb.ToString();
        PoolUtility.Free(ref sb);

        return text;
    }
}