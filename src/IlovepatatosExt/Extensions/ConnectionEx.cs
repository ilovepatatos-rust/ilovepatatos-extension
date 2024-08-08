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
    
    public static void ChatMessageAsCopyable(this Connection connection, string msg, ulong steam64 = 0)
    {
        if (!string.IsNullOrEmpty(msg))
            ConsoleNetwork.SendClientCommand(connection, "chat.add2", ConVar.Chat.ChatChannel.Global, steam64, msg);
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

        RpcTarget target = RpcTarget.Player("AddUI", connection);
        CommunityEntity.ServerInstance.ClientRPC(target, json);
    }

    public static void AddUi(this List<Connection> connections, string json)
    {
        if (connections.Count == 0 || string.IsNullOrEmpty(json))
            return;

        RpcTarget target = RpcTarget.Players("AddUI", connections);
        CommunityEntity.ServerInstance.ClientRPC(target, json);
    }

    public static void DestroyUi(this Connection connection, string name)
    {
        if (string.IsNullOrEmpty(name))
            return;
        
        RpcTarget target = RpcTarget.Player("DestroyUI", connection);
        CommunityEntity.ServerInstance.ClientRPC(target, name);
    }

    public static void DestroyUi(this List<Connection> connections, string name)
    {
        if (connections.Count == 0 || string.IsNullOrEmpty(name))
            return;
        
        RpcTarget target = RpcTarget.Players("DestroyUI", connections);
        CommunityEntity.ServerInstance.ClientRPC(target, name);
    }
}