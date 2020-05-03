using System;
using System.Collections.Generic;
using System.Linq;
using Pof.Internal;

namespace Pof
{
    public class EntityManager<TEntity>
        : IEntityManager<TEntity>
        , IMessageHandler
    {
        private IMessagePump? _messagePump;
        private readonly Dictionary<string, IMessageHandler> _handlers = new Dictionary<string, IMessageHandler>();
        public TEntity Entity { get; }

        public EntityManager(TEntity entity)
        {
            Entity = entity;
        }
        
        public void Connect(IMessagePump messagePump)
        {
            _messagePump = messagePump;
        }
        
        public void HandleMessage(Message message)
        {
            if (!_handlers.ContainsKey(message.PropertyName))
            {
                _handlers.Add(message.PropertyName, new PropertyHandler(Entity, message.PropertyName));
            }
            
            _handlers[message.PropertyName].HandleMessage(message);
        }

        public void Handle(IEnumerable<Message> messages)
        {
            foreach (var message in messages)
            {
                HandleMessage(message);
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
                var originalValue = (object?)null;
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
