using JetBrains.Annotations;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class ChatMsgEx
{
    [MustUseReturnValue]
    public static float TotalTime(this IEnumerable<ChatMsg> messages)
    {
        return messages.Sum(message => message.SecondsBefore + message.SecondsAfter);
    }
}