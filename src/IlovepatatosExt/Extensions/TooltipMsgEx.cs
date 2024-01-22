using JetBrains.Annotations;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class TooltipMsgEx
{
    public static float TotalTime(this IEnumerable<TooltipMsg> messages)
    {
        return messages.Sum(message => message.SecondsBefore + message.SecondsAfter);
    }
}