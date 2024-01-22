using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Oxide.Ext.IlovepatatosExt;

[Serializable]
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class TooltipMsg
{
    [JsonConverter(typeof(StringEnumConverter))]
    [JsonProperty("Style [Blue_Short, Blue_Normal, Blue_Long, Red_Normal, Server_Event]")]
    public GameTip.Styles Style = GameTip.Styles.Blue_Normal;

    [JsonProperty("Seconds before")]
    public float SecondsBefore;

    [JsonProperty("Seconds after")]
    public float SecondsAfter;

    [JsonProperty("Message")]
    public string Msg = "";
}