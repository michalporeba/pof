using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Pof.Internal
{
    internal class PropertyHandler : IMessageHandler
    {
        private readonly object _object;
        private readonly PropertyInfo _property;
        private readonly List<Candidate> _candidates = new List<Candidate>();
        private readonly List<string> _predecessors = new List<string>();
        private readonly Queue<Message> _outgoingQueue = new Queue<Message>();
        private bool _isDraft = true;

        public PropertyHandler(object obj, string propertyName)
        {
            _object = obj;
            _property = _object.GetType().GetProperty(propertyName);
        }

        public void Commit()
        {
            var currentValue = _property.GetValue(_object);
            var isChanged = _candidates.Any(
                c => c.Value == null && currentValue == null 
                     || currentValue != null && !currentValue.Equals(c.Value)
                     );
            
            if (isChanged || _isDraft)
                SetTo(currentValue);

            _isDraft = false;
        }

        public void HandleMessage(Message message)
        {
            if (!_predecessors.Contains(message.Hash))
            {
                _candidates.Add(new Candidate(message.Hash, message.Value));
                _property.SetValue(_object, message.Value);
            }

            var newPredecessors = message.Predecessors.Except(_predecessors);
            _candidates.RemoveAll(c => newPredecessors.Contains(c.MessageHash));
            _predecessors.AddRange(message.Predecessors);
        }

        public bool HasMessagesInQueue() => _outgoingQueue.Count > 0;

        public Message GetNextMessage() => _outgoingQueue.Dequeue();
        
        public bool HasConflicts() =>  _candidates.Count > 1;
        
        private void SetTo(object? value)
        {
            var message = new Message(
                _property.Name,
                _candidates.Select(c => c.MessageHash).ToArray(),
                value);
            
            _outgoingQueue.Enqueue(message);
            HandleMessage(message);
        }
    }
}