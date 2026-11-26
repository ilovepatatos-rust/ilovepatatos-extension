using JetBrains.Annotations;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class ChatMsgEx
{
    extension(IEnumerable<ChatMsg> messages)
    {
        [MustUseReturnValue]
        public float TotalTime()
        {
            return messages.Sum(message => message.SecondsBefore + message.SecondsAfter);
        }
    }
}