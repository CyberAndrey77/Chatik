using Client.Models;
using Client.Services.EventArgs;
using Common.EventArgs;
using System;
using System.Collections.Generic;

namespace Client.Services
{
    public interface IChatService
    {
        EventHandler<ChatEventArgs> ChatIsCreatedEvent { get; set; }
        EventHandler<UserChatEventArgs<Chat>> GetUserChats { get; set; }
        EventHandler<ChatEventArgs> ChatIsCreated { get; set; }
        EventHandler<ChatEventArgs> CreatedChat { get; set; }
        void CreateChat(string chatName, int chatId, string creator, List<int> invented, bool isDialog);
        void Subscribe();
    }
}
