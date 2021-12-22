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
        EventHandler<ChatEventArgs> ChatEvent { get; set; }
        void CreateDialog(string creator, string invented);
    }
}
