using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Services.EventArgs
{
    public class GetUserEventArgs : System.EventArgs
    {
        public string Login { get; set; }
        public bool IsConnect { get; set; }
        public int Id { get; set; }

        public GetUserEventArgs(string login, bool isConnect, int id)
        {
            Login = login;
            IsConnect = isConnect;
            Id = id;
        }
    }
}
