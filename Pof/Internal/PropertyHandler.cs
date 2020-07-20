using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace Pof.Internal
{
    internal class PropertyHandler : IPropertySetter
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

        public void AddState(object? value, string hash, ImmutableSortedSet<string> predecessors)
        {
            if (!_predecessors.Contains(hash))
            {
                _candidates = _candidates.Add(new Candidate(hash, value));
                _property.SetValue(_object, value);
            }

            var newPredecessors = predecessors.Except(_predecessors);
            _candidates = _candidates.RemoveAll(c => newPredecessors.Contains(c.MessageHash));

            _predecessors = _predecessors.AddRange(predecessors);
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
            message.ApplyWith(this);
        }
    }
}