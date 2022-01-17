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
        public EventHandler<ChatEventArgs> ChatCreatedEvent { get; set; }
        public EventHandler<ChatEventArgs> ChatIsCreatedEvent { get; set; }

        public void CreateChat(string chatName, string creator, List<int> invented, bool isDialog)
        {
            _connectionService.CreateChat(chatName, creator, invented, isDialog);
        }

        public ChatService(IConnectionService connectionService)
        {
            _connectionService = connectionService;
            _connectionService.ChatCreated += OnChatCreated;
            _connectionService.ChatIsCreatedEvent += OnChatIsCreated;
        }

        private void OnChatIsCreated(object sender, ChatEventArgs e)
        {
            ChatIsCreatedEvent?.Invoke(this, e);
        }

        private void OnChatCreated(object sender, ChatEventArgs e)
        {
            ChatCreatedEvent?.Invoke(this, e);
        }


    }
}
