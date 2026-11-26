using JetBrains.Annotations;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class TooltipMsgEx
{
    extension(IEnumerable<TooltipMsg> messages)
    {
        [MustUseReturnValue]
        public float TotalTime()
        {
            return messages.Sum(message => message.SecondsBefore + message.SecondsAfter);
        }
    }
}