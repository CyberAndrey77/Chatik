using System;
using System.Collections.Generic;
using System.Text;
using Client.Models;
using Client.NetWork;
using Client.NetWork.EventArgs;
using Client.Services.EventArgs;
using Common.EventArgs;

namespace Client.Services
{
    public interface IMessageService
    {
        EventHandler<MessageEventArgs> MessageEvent { get; set; }
        EventHandler<ChatMessageEventArgs> ChatMessageEvent { get; set; }
        EventHandler<GetMessagesEventArgs<Message>> GetMessagesEvent { get; set; }
        EventHandler<MessageRequestEvent> MessageStatusChangeEvent { get; set; }
        
        void SendChatMessage(int senderUserId, string text, int chatId, List<int> userIds, bool isDialog, Guid messageId);
        void GetMessages(int chatId);
        void Subscribe();
    }
}