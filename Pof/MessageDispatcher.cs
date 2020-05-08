using System;
using System.Collections.Immutable;

namespace Pof
{
    public class MessageDispatcher
    {
        private ImmutableDictionary<string, SingleTopicDispatcher> _subscriptions;

        public MessageDispatcher()
        {
            _subscriptions = ImmutableDictionary<string, SingleTopicDispatcher>.Empty;
        }
        
        public void Dispatch(string topic, Message message)
        {
            if (!_subscriptions.ContainsKey(topic)) return;

            _subscriptions[topic].Dispatch(message);
        }

        public void Subscribe(IMessageHandler handler, string topic)
        {
            if (!_subscriptions.ContainsKey(topic))
                _subscriptions = _subscriptions.Add(topic, new SingleTopicDispatcher());
            
            _subscriptions[topic].Subscribe(handler);
        }

        private class SingleTopicDispatcher
        {
            private ImmutableDictionary<Guid, IMessageHandler> _subscriptions;

            public SingleTopicDispatcher()
            {
                _subscriptions = ImmutableDictionary<Guid, IMessageHandler>.Empty;
            }
            
            public void Dispatch(Message message)
            {
                foreach(var subscription in _subscriptions)
                    subscription.Value.HandleMessage(message);
            }
            
            public void Subscribe(IMessageHandler handler)
            {
                if (!_subscriptions.ContainsKey(handler.Id))
                    _subscriptions = _subscriptions.Add(handler.Id, handler);
            }
        }
    }
}