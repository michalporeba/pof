using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace Pof.Internal
{
    internal class PropertyHandler
    {
        private readonly object _object;
        private readonly PropertyInfo _property;
        private ImmutableList<Candidate> _candidates;
        private ImmutableList<string> _predecessors;
        private ImmutableQueue<Message> _outgoingQueue;
        private bool _isDraft = true;

        public PropertyHandler(object obj, string propertyName)
        {
            _object = obj;
            _property = _object.GetType().GetProperty(propertyName);
            _candidates = ImmutableList<Candidate>.Empty;
            _predecessors = ImmutableList<string>.Empty;
            _outgoingQueue = ImmutableQueue<Message>.Empty;
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
                _candidates = _candidates.Add(new Candidate(message.Hash, message.Value));
                _property.SetValue(_object, message.Value);
            }

            var newPredecessors = message.Predecessors.Except(_predecessors);
            _candidates = _candidates.RemoveAll(c => newPredecessors.Contains(c.MessageHash));
            
            _predecessors = _predecessors.AddRange(message.Predecessors);
        }

        public IEnumerable<Message> GetMessages()
        {
            var output = _outgoingQueue.ToImmutableArray();
            _outgoingQueue = _outgoingQueue.Clear();
            return output;
        }

        public bool HasConflicts() =>  _candidates.Count > 1;
        
        private void SetTo(object? value)
        {
            var message = new Message(
                _property.Name,
                _candidates.Select(c => c.MessageHash).ToArray(),
                value);
            
            _outgoingQueue = _outgoingQueue.Enqueue(message);
            HandleMessage(message);
        }
    }
}