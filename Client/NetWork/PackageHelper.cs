using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Client.Enums;
using Client.Models;
using Common;

namespace Client.NetWork
{
    public class PackageHelper
    {
        public Dictionary<string, EnumKey> Keys;

        public PackageHelper()
        {
            Keys = new Dictionary<string, EnumKey>()
            {
                {nameof(ConnectionRequest), EnumKey.ConnectionKeyConnection},
                {nameof(ConnectedUser), EnumKey.ConnectionKeyConnectedUser},
                {nameof(MessageRequest), EnumKey.MessageKeyRequestMessage},
                {nameof(CreateChatResponse), EnumKey.ChatKeyCreateChat},
                {nameof(ChatMessageResponseServer), EnumKey.MessageKeyOnChatMessage},
                {nameof(CreateChatRequest), EnumKey.ChatKeyChatIsCreate},
                {nameof(UserChats<Chat>), EnumKey.ChatKeyGetChats},
                {nameof(GetMessageRequest<Message>), EnumKey.MessageKeyGetMessages},
                {nameof(GetLogsRequest<Log>), EnumKey.LogKey}
            };
        }
    }
}
