using System.Collections.Generic;
using System.Linq;

namespace Pof
{
    public class PofHandler<TEntity>
        where TEntity : IPofEntity, new()
    {
        private readonly TEntity _original = new TEntity();
        private readonly Dictionary<string, List<Candidate>> _candidates = new Dictionary<string, List<Candidate>>();
        private readonly Dictionary<string, List<string>> _predecessors = new Dictionary<string, List<string>>();

        public TEntity Entity { get; } = new TEntity();
        public void Handle(Message message)
        {
            // ensure we track candidates for every value
            if (!_candidates.ContainsKey(message.PropertyName))
            {
                _candidates.Add(message.PropertyName, new List<Candidate>());
            }

            if (!_predecessors.ContainsKey(message.PropertyName))
            {
                _predecessors.Add(message.PropertyName, new List<string>());
            }

            if (!_predecessors[message.PropertyName].Contains(message.Hash))
            {
                _candidates[message.PropertyName].Add(new Candidate(message.Hash, message.Value));
                // set the value
                var property = Entity.GetType().GetProperty(message.PropertyName);
                property.SetValue(Entity, message.Value);
            }

            var newPredecessors = message.Predecessors.Except(_predecessors[message.PropertyName]);
            _candidates[message.PropertyName].RemoveAll(c => newPredecessors.Contains(c.MessageHash));
            
            _predecessors[message.PropertyName].AddRange(message.Predecessors);
        }

        public void Handle(IEnumerable<Message> messages)
        {
            foreach (var message in messages)
            {
                Handle(message);
            }
        }

        public bool HasConflicts()
        {
            return _candidates.Any(p => p.Value.Count > 1);
        }

        public IEnumerable<Message> GetNewMessages()
        {
            foreach(var property in Entity.GetType().GetProperties())
            {
                var originalValue = property.GetValue(_original);
                var newValue = property.GetValue(Entity);

                if (originalValue == null && newValue != null
                    || originalValue != null && !originalValue.Equals(newValue)
                )
                {
                    yield return new Message(property.Name, newValue);
                }
            }
        }
    }
}
