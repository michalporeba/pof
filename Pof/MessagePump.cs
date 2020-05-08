using System.Collections.Generic;
using Pof.Data;

namespace Pof
{
    public class MessagePump : IMessagePump
    {
        private readonly IMessageStore _store;
        private Dictionary<string, List<IMessageHandler>> _handlers = new Dictionary<string, List<IMessageHandler>>();
        
        public MessagePump(IMessageStore store)
        {
            _store = store;
        }
        
        public void Connect(string topic, IMessageHandler handler)
        {
            if (!_handlers.ContainsKey(topic))
                _handlers.Add(topic, new List<IMessageHandler>());
            
            _handlers[topic].Add(handler);
        }

        public void Push(string topic, Message message)
        {
            _store.Save(topic, message);
            if (!_handlers.ContainsKey(topic)) return;
            
            foreach(var handler in _handlers[topic])
                handler.HandleMessage(message);
        }
    }
}