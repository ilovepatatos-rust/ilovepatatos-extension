using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class StringEx
{
    private static readonly Regex s_placeholderRegex = new(@"\{(\d+)(?:\:([^}]+))?\}", RegexOptions.Compiled);

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
            return s_placeholderRegex.Replace(format, match => EvaluatePlaceholder(match, args));
        }
    }

    private static string EvaluatePlaceholder(Match match, object[] args)
    {
        Group indexGroup = match.Groups[1];
        Group formatGroup = match.Groups[2];

        if (!int.TryParse(indexGroup.Value, out int index))
            return match.Value;

        if (index < 0 || index >= args.Length)
            return match.Value;

        object arg = args[index];
        if (arg == null)
            return string.Empty;

        if (!formatGroup.Success)
            return arg.ToString();

        try
        {
            return string.Format($"{{0:{formatGroup.Value}}}", arg);
        }
        catch
        {
            return arg.ToString();
        }
    }
}