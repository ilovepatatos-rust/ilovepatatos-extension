using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Oxide.Ext.IlovepatatosExt;

[Serializable]
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class ChatMsg
{
    [JsonProperty("Seconds before")]
    public float SecondsBefore;

    [JsonProperty("Seconds after")]
    public float SecondsAfter;

    [JsonProperty("Message")]
    public string Msg = "";
}