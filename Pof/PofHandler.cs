using System.Collections.Generic;
using System.Linq;
using Pof.Internal;

namespace Pof
{
    public class PofHandler<TEntity>
        where TEntity : IPofEntity, new()
    {
        private readonly TEntity _original = new TEntity();
        private readonly Dictionary<string, IHandler> _handlers = new Dictionary<string, IHandler>();
        public TEntity Entity { get; } = new TEntity();
        public void Handle(Message message)
        {
            if (!_handlers.ContainsKey(message.PropertyName))
            {
                _handlers.Add(message.PropertyName, new PropertyHandler(Entity, message.PropertyName));
            }
            
            _handlers[message.PropertyName].Handle(message);
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
            return _handlers.Any(x => x.Value.HasConflicts());
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
