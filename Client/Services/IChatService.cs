using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Services.EventArgs;

namespace Client.Services
{
    public interface IChatService
    {
        EventHandler<ChatEventArgs> ChatCreatedEvent { get; set; }
        EventHandler<ChatEventArgs> ChatIsCreatedEvent { get; set; }
        void CreateChat(string chatName,string creator, List<int> invented, bool isDialog);
    }
}
