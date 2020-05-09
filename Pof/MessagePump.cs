using Pof.Data;

namespace Pof
{
    public class MessagePump : IMessagePump
    {
        private readonly IMessageStore _store;
        private readonly MessageDispatcher _dispatcher = new MessageDispatcher();
        
        public MessagePump(IMessageStore store)
        {
            _store = store;
        }
        
        public void Connect(string topic, IMessageHandler handler)
        {
            _dispatcher.Subscribe(handler, topic);
            foreach (var message in _store.GetAllFromTopic(topic))
            {
                handler.HandleMessage(message);
            }
        }

        public void Push(string topic, Message message)
        {
            _store.Save(topic, message);
            _dispatcher.Dispatch(topic, message);
        }
    }
}