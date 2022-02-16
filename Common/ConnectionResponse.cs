namespace Common
{
    public class ConnectionResponse
    {
        public string Login { get; set; }

        public ConnectionResponse(string login)
        {
            Login = login;
        }

        public MessageContainer GetContainer()
        {
            var container = new MessageContainer
            {
                Identifier = nameof(ConnectionResponse),
                Payload = this
            };

            return container;
        }
    }
}
