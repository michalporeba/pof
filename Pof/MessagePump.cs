using Pof.Data;

namespace Pof
{
    public class MessagePump : IMessagePump
    {
        private readonly IMessageStore _store;
        
        public MessagePump(IMessageStore store)
        {
            _store = store;
        }
        
        public void Connect(string topic, IMessageHandler handler)
        {
            throw new System.NotImplementedException();
        }

        public void Push(string topic, Message message)
        {
            throw new System.NotImplementedException();
        }
    }
}