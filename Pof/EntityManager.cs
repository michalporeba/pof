using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Pof.Internal;

namespace Pof
{
    public class EntityManager<TEntity>
        : IEntityManager<TEntity>
        where TEntity : notnull
    {
        private IMessagePump? _messagePump;
        private readonly Dictionary<string, PropertyHandler> _handlers = new Dictionary<string, PropertyHandler>();
        
        public TEntity Entity { get; }
        public string Topic { get; }

        internal EntityManager(TEntity entity, string topic)
        {
            Entity = entity;
            Topic = topic;

            foreach (var property in entity.GetType().GetProperties(BindingFlags.Public))
                _handlers.Add(property.Name, new PropertyHandler(Entity, property.Name));   
        }

        public void Commit()
        {
            throw new NotImplementedException();
        }
        
        public void Connect(IMessagePump messagePump)
        {
            _messagePump = messagePump;
        }
        
        public void HandleMessage(Message message)
        {
            if (!_handlers.ContainsKey(message.PropertyName))
                _handlers.Add(message.PropertyName, new PropertyHandler(Entity, message.PropertyName));
            
            _handlers[message.PropertyName].HandleMessage(message);
        }

        public bool HasConflicts()
        {
            return _handlers.Any(x => x.Value.HasConflicts());
        }
    }
}
