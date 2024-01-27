using JetBrains.Annotations;
using Network;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class ConnectionEx
{
    public static bool IsModerator(this Connection connection)
    {
        return connection.authLevel > 0;
    }

    public static void ChatMessage(this List<Connection> connections, string msg, ulong steam64 = 0)
    {
        if (connections.Count > 0 && !string.IsNullOrEmpty(msg))
            ConsoleNetwork.SendClientCommand(connections, "chat.add", ConVar.Chat.ChatChannel.Global, steam64, msg);
    }

    public static void ChatMessageAsCopyable(this List<Connection> connections, string msg, ulong steam64 = 0)
    {
        if (connections.Count > 0 && !string.IsNullOrEmpty(msg))
            ConsoleNetwork.SendClientCommand(connections, "chat.add2", ConVar.Chat.ChatChannel.Global, steam64, msg);
    }

    public static void ShowToast(this List<Connection> connections, GameTip.Styles style, Translate.Phrase phrase, params string[] arguments)
    {
        if (connections.Count > 0 && !string.IsNullOrEmpty(phrase.english))
            ConsoleNetwork.SendClientCommand(connections, "gametip.showtoast_translated", (int)style, phrase.token, phrase.english, arguments);
    }

    public static void AddUi(this Connection connection, string json)
    {
        if (string.IsNullOrEmpty(json))
            return;

        var info = new SendInfo(connection);
        CommunityEntity.ServerInstance.ClientRPCEx(info, null, "AddUI", json);
    }

    public static void AddUi(this List<Connection> connections, string json)
    {
        if (connections.Count == 0 || string.IsNullOrEmpty(json))
            return;

        var info = new SendInfo(connections);
        CommunityEntity.ServerInstance.ClientRPCEx(info, null, "AddUI", json);
    }

    public static void DestroyUi(this Connection connection, string name)
    {
        if (string.IsNullOrEmpty(name))
            return;

        var info = new SendInfo(connection);
        CommunityEntity.ServerInstance.ClientRPCEx(info, null, "DestroyUI", name);
    }

    public static void DestroyUi(this List<Connection> connections, string name)
    {
        if (connections.Count == 0 || string.IsNullOrEmpty(name))
            return;

        var info = new SendInfo(connections);
        CommunityEntity.ServerInstance.ClientRPCEx(info, null, "DestroyUI", name);
    }
}