using Client.Services.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Services
{
    public class ChatService : IChatService
    {
        private readonly IConnectionService _connectionService;
        public EventHandler<ChatEventArgs> ChatEvent { get; set; }

        public void CreateChat(string chatName, string creator, List<string> invented)
        {
            _connectionService.CreateChat(chatName, creator, invented);
        }

        public ChatService(IConnectionService connectionService)
        {
            _connectionService = connectionService;
            _connectionService.ChatCreated += OnChatCreated;
        }

        private void OnChatCreated(object sender, ChatEventArgs e)
        {
            ChatEvent?.Invoke(this, e);
        }
    }
}
