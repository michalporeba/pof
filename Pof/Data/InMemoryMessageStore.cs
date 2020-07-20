using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Pof.Data
{
    public class InMemoryMessageStore : IMessageStore
    {
        private readonly Dictionary<string, IList<Message>> _topics = new Dictionary<string, IList<Message>>();

        public bool Contains(string topic, Message message)
        {
            return _topics.ContainsKey(topic) 
                   && _topics[topic].Any(m => m.Equals(message));
        }
        
        public void Save(string topic, Message message)
        {
            if (!_topics.ContainsKey(topic))
                _topics.Add(topic, new List<Message>());
            
            if (_topics[topic].All(m => !m.Equals(message)))
               _topics[topic].Add(message);
        }

        public IImmutableList<Message> GetAllFromTopic(string topic)
            => _topics.ContainsKey(topic)
                ? _topics[topic].ToImmutableList()
                : ImmutableList<Message>.Empty;
    }
}