using Client.Enums;
using Client.Models;
using Common;
using System.Collections.Generic;

namespace Client.NetWork
{
    public class PackageHelper : IPackageHelper
    {
        public Dictionary<string, EnumKey> Keys { get; set; }

        public PackageHelper()
        {
            Keys = new Dictionary<string, EnumKey>()
            {
                {nameof(ConnectionRequest), EnumKey.ConnectionKeyConnection},
                {nameof(ConnectedUser), EnumKey.ConnectionKeyOnlineUsers},
                {nameof(MessageRequest), EnumKey.MessageKeyRequestMessage},
                {nameof(CreateChatResponse), EnumKey.ChatKeyCreateChat},
                {nameof(ChatMessageResponseServer), EnumKey.MessageKeyOnChatMessage},
                {nameof(CreateChatRequest), EnumKey.ChatKeyChatIsCreate},
                {nameof(UserChats<Chat>), EnumKey.ChatKeyGetChats},
                {nameof(GetMessageRequest<Message>), EnumKey.MessageKeyGetMessages},
                {nameof(GetLogsRequest<Log>), EnumKey.LogKey},
                {nameof(GetAllUsers), EnumKey.ConnectionKeyAllUsers}
            };
        }
    }
}
