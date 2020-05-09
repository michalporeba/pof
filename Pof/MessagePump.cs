using System.Collections.Immutable;
using Pof.Data;

namespace Pof
{
    public class MessagePump : IMessagePump
    {
        private readonly IMessageStore _store;
        private readonly MessageDispatcher _dispatcher = new MessageDispatcher();
        private ImmutableList<IMessagePumpClient> _remotes;
        
        public MessagePump(IMessageStore store)
        {
            _store = store;
            _remotes = ImmutableList<IMessagePumpClient>.Empty;
        }

        public void Connect(IMessagePumpClient messagePumpClient)
        {
            _remotes = _remotes.Add(messagePumpClient);
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
            foreach (var remote in _remotes)
            {
                remote.Push(topic, message);
            }
        }
    }
}