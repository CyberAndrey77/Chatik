using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Models;
using Client.Services.EventArgs;
using Common.EventArgs;

namespace Client.Services
{
    public interface IChatService
    {
        EventHandler<ChatEventArgs> ChatIsCreatedEvent { get; set; }
        EventHandler<UserChatEventArgs<Chat>> GetUserChats { get; set; }
        EventHandler<ChatEventArgs> ChatIsCreated { get; set; }
        EventHandler<ChatEventArgs> CreatedChat { get; set; }
        void CreateChat(string chatName,int chatId, string creator, List<int> invented, bool isDialog);
        void Subscribe();
    }
}
