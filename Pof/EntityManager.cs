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

            foreach (var property in GetManagedProperties())
                _handlers.Add(property.Name, new PropertyHandler(Entity, property.Name));   
        }

        public void Commit()
        {
            foreach (var handler in _handlers.Values)
            {
                handler.Commit();
                while(handler.HasMessagesInQueue())
                    _messagePump?.Push(Topic, handler.GetNextMessage());
            }
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

        private IEnumerable<PropertyInfo> GetManagedProperties()
            => Entity.GetType().GetProperties().Where(p => p.CanRead && p.CanWrite);
    }
}
