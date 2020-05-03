using System;
using System.Collections.Generic;
using System.Linq;
using Pof.Internal;

namespace Pof
{
    public class EntityManager<TEntity>
        : IEntityManager<TEntity>
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

        public bool HasConflicts()
        {
            return _handlers.Any(x => x.Value.HasConflicts());
        }
    }
}
