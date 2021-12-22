using Client.Services.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Services
{
    public class ClassService : IChatService
    {
        private readonly IConnectionService _connectionService;
        public EventHandler<ChatEventArgs> ChatEvent { get; set; }

        public void CreateDialog(string creator, string invented)
        {
            _connectionService.CreateDialog(creator, invented);
        }

        public ClassService(IConnectionService connectionService)
        {
            _connectionService = connectionService;
        }
    }
}
