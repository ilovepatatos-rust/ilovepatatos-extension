using JetBrains.Annotations;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class StringEx
{
    public static string FormatNoThrow(this string format, params object[] args)
    {
        for (int i = 0; i < args.Length; i++)
            format = format.Replace("{" + i + "}", args[i].ToString());

        return format;
    }
}