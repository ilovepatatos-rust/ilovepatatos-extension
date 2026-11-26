using JetBrains.Annotations;
using Network;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class ConnectionEx
{
    extension(Connection connection)
    {
        [MustUseReturnValue]
        public bool IsModerator()
        {
            return connection.authLevel > 0;
        }

        public void ChatMessageAsCopyable(string msg, ulong steam64 = 0)
        {
            if (!string.IsNullOrEmpty(msg))
                ConsoleNetwork.SendClientCommand(connection, "chat.add2", ConVar.Chat.ChatChannel.Global, steam64, msg);
        }

        public void AddUi(string json)
        {
            if (string.IsNullOrEmpty(json))
                return;

            RpcTarget target = RpcTarget.Player("AddUI", connection);
            CommunityEntity.ServerInstance.ClientRPC(target, json);
        }

        public void DestroyUi(string name)
        {
            if (string.IsNullOrEmpty(name))
                return;

            RpcTarget target = RpcTarget.Player("DestroyUI", connection);
            CommunityEntity.ServerInstance.ClientRPC(target, name);
        }
    }

    extension(List<Connection> connections)
    {
        public void ChatMessage(string msg, ulong steam64 = 0)
        {
            if (connections.Count > 0 && !string.IsNullOrEmpty(msg))
                ConsoleNetwork.SendClientCommand(connections, "chat.add", ConVar.Chat.ChatChannel.Global, steam64, msg);
        }

        public void ChatMessageAsCopyable(string msg, ulong steam64 = 0)
        {
            if (connections.Count > 0 && !string.IsNullOrEmpty(msg))
                ConsoleNetwork.SendClientCommand(connections, "chat.add2", ConVar.Chat.ChatChannel.Global, steam64, msg);
        }

        public void ShowToast(GameTip.Styles style, Translate.Phrase phrase, bool overlay = false, params string[] arguments)
        {
            if (connections.Count > 0 && !string.IsNullOrEmpty(phrase.english))
                ConsoleNetwork.SendClientCommand(connections, "gametip.showtoast_translated", (int)style, phrase.token, phrase.english, overlay,
                    arguments);
        }

        public void AddUi(string json)
        {
            if (connections.Count == 0 || string.IsNullOrEmpty(json))
                return;

            RpcTarget target = RpcTarget.Players("AddUI", connections);
            CommunityEntity.ServerInstance.ClientRPC(target, json);
        }

        public void DestroyUi(string name)
        {
            if (connections.Count == 0 || string.IsNullOrEmpty(name))
                return;

            RpcTarget target = RpcTarget.Players("DestroyUI", connections);
            CommunityEntity.ServerInstance.ClientRPC(target, name);
        }
    }
}