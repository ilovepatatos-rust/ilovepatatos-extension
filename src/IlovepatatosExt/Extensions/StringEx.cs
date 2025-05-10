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

        for (int i = 0; i < args.Length; i++)
            format = format.Replace("{" + i + "}", $"{args[i]}");

        return format;
    }
}