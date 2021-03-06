namespace Common
{
    public class GetMessageResponse
    {
        public int ChatId { get; set; }

        public GetMessageResponse(int chatId)
        {
            ChatId = chatId;
        }

        public virtual MessageContainer GetContainer()
        {
            var container = new MessageContainer
            {
                Identifier = nameof(GetMessageResponse),
                Payload = this
            };

            return container;
        }
    }
}
