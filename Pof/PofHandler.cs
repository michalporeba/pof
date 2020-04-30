using System.Collections.Generic;
using System.Linq;

namespace Pof
{
    public class PofHandler<TEntity>
        where TEntity : IPofEntity, new()
    {
        private readonly TEntity _original = new TEntity();
        private readonly Dictionary<string, List<Candidate>> _candidates = new Dictionary<string, List<Candidate>>();

        public TEntity Entity { get; } = new TEntity();
        public void Handle(Message message)
        {
            if (!_candidates.ContainsKey(message.PropertyName))
            {
                _candidates.Add(message.PropertyName, new List<Candidate>());
            }
            
            _candidates[message.PropertyName].Add(new Candidate(message.Hash, message.Value));
            
            var property = Entity.GetType().GetProperty(message.PropertyName);
            property.SetValue(Entity, message.Value);
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
