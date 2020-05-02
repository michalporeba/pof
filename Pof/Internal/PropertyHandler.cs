using System.Collections.Generic;
using System.Reflection;

namespace Pof.Internal
{
    internal class PropertyHandler : IHandler
    {
        private readonly object _object;
        private readonly PropertyInfo _property;
        private readonly List<Candidate> _candidates = new List<Candidate>();
        private readonly List<string> _predecessors = new List<string>();

        public PropertyHandler(object obj, string propertyName)
        {
            _object = obj;
            _property = _object.GetType().GetProperty(propertyName);
        }
        
        public void Handle(Message message)
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

        public bool HasConflicts() =>  _candidates.Count > 1;
    }
}