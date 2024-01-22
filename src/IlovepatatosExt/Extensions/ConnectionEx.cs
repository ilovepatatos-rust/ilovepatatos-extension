using JetBrains.Annotations;
using Network;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class ConnectionEx
{
    public static void ChatMessage(this List<Connection> connections, string msg, ulong steam64 = 0)
    {
        if (connections.Count > 0 && !string.IsNullOrEmpty(msg))
            ConsoleNetwork.SendClientCommand(connections, "chat.add", ConVar.Chat.ChatChannel.Global, steam64, msg);
    }

    public static void ShowToast(this List<Connection> connections, GameTip.Styles style, Translate.Phrase phrase, params string[] arguments)
    {
        if (connections.Count > 0 && !string.IsNullOrEmpty(phrase.english))
            ConsoleNetwork.SendClientCommand(connections, "gametip.showtoast_translated", (int)style, phrase.token, phrase.english, arguments);
    }
}