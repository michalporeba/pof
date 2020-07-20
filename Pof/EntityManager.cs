using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Pof.Internal;

namespace Pof
{
    /// <summary>
    /// Manager of entities responsible for connecting
    /// the entity to the wider system.
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity to be managed</typeparam>
    public class EntityManager<TEntity>
        : IMessageHandler
        where TEntity : notnull
    {
        private IMessagePump? _messagePump;
        private readonly Dictionary<string, PropertyHandler> _handlers = new Dictionary<string, PropertyHandler>();

        /// <inheritdoc cref="IMessageHandler"/>
        public Guid Id { get; } = Guid.NewGuid();
        
        /// <summary>
        /// The managed entity
        /// </summary>
        public TEntity Entity { get; }
        
        /// <summary>
        /// Topic to which the manager (and the Entity) are subscribed
        /// </summary>
        public string Topic { get; }

        /// <summary>
        /// Create an instance of EntityManager
        /// </summary>
        /// <param name="entity">Entity instance to be managed</param>
        /// <param name="topic">Topic to which the entity belongs</param>
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
                var newMessages = handler.GetMessages();
                foreach (var message in newMessages)
                    _messagePump?.Push(Topic, message);
            }
        }
        
        public void Connect(IMessagePump messagePump)
        {
            _messagePump = messagePump;
        }

        public void RouteMessage(Message message, string propertyName)
        {
            if (!_handlers.ContainsKey(propertyName))
                _handlers.Add(propertyName, new PropertyHandler(Entity, propertyName));
            
            message.ApplyWith(_handlers[propertyName]);
        }
        
        /// <inheritdoc cref="IMessageHandler"/>
        public bool HasConflicts()
        {
            return _handlers.Any(x => x.Value.HasConflicts());
        }

        private IEnumerable<PropertyInfo> GetManagedProperties()
            => Entity.GetType().GetProperties().Where(p => p.CanRead && p.CanWrite);
    }
}
