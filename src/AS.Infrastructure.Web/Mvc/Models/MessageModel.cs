namespace AS.Infrastructure.Web.Mvc
{
    public enum MessageElementType
    {
        CallOut,
        Alert
    }

    public class MessageModel : ASModelBase
    {
        public MessageElementType ElementType { get; private set; }
        public MessageType MessageType { get; private set; }
        public string Message { get; private set; }

        public MessageModel(MessageType messageType, MessageElementType elementType, string message)
        {
            this.MessageType = messageType;
            this.ElementType = elementType;
            this.Message = message;
        }
    }
}