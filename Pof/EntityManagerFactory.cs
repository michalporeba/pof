using System;

namespace Pof
{
    public static class EntityManagerFactory
    {
        public static EntityManager<TEntity> Create<TEntity>(TEntity entity, IMessagePump messagePump)
            where TEntity : notnull
        {
            var idProperty = entity.GetType().GetProperty("Id");
            if (idProperty == null)
            {
                throw new InvalidOperationException("Entity has no ID and topic is not provided.");
            }

            return Create(entity, messagePump, idProperty.GetValue(entity).ToString());
        }
        
        public static EntityManager<TEntity> Create<TEntity>(TEntity entity, IMessagePump pump, string topic)
            where TEntity : notnull
        {
            if (string.IsNullOrWhiteSpace(topic))
            {
                throw new ArgumentOutOfRangeException(nameof(topic), $"{nameof(topic)} cannot be empty!");
            }
            
            var manager = new EntityManager<TEntity>(entity, topic);
            manager.Connect(pump);
            pump.Connect(topic ?? string.Empty, manager);
            return manager;
        }

        public static EntityManager<TEntity> Create<TEntity>(TEntity entity, string idPropertyName, IMessagePump pump)
            where TEntity : notnull
        {
            var idProperty = entity.GetType().GetProperty(idPropertyName)
                             ?? throw new ArgumentOutOfRangeException(nameof(idPropertyName),
                                 $"Property {idPropertyName} does not exist on {entity.GetType()}");
            var topic = idProperty.GetValue(entity).ToString();
            return Create(entity, pump, topic);
        }
    }
}