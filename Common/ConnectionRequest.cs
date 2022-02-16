using Common.Enums;

namespace Common
{
    public class ConnectionRequest
    {
        public string Login { get; set; }

        public ConnectionRequestCode CodeConnected
        {
            get;
            set;
        }

        public int Id { get; set; }

        public ConnectionRequest(string login, ConnectionRequestCode codeConnected, int id)
        {
            Login = login;
            CodeConnected = codeConnected;
            Id = id;
        }

        public MessageContainer GetContainer()
        {
            var container = new MessageContainer
            {
                Identifier = nameof(ConnectionRequest),
                Payload = this
            };

            return container;
        }
    }
}
