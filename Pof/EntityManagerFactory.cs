using System;

namespace Pof
{
    public static class EntityManagerFactory
    {
        public static IEntityManager<TEntity> Create<TEntity>(TEntity entity, IMessagePump messagePump)
            where TEntity : notnull
        {
            var idProperty = entity.GetType().GetProperty("Id");
            if (idProperty == null)
            {
                throw new InvalidOperationException("Entity has no ID and topic is not provided.");
            }

            return Create<TEntity>(entity, messagePump, idProperty.GetValue(entity).ToString());
        }
        
        public static IEntityManager<TEntity> Create<TEntity>(TEntity entity, IMessagePump messagePump, string topic)
            where TEntity : notnull
        {
            var manager = new EntityManager<TEntity>(entity, topic);
            manager.Connect(messagePump);
            messagePump.Subscribe(topic ?? string.Empty, manager);
            return manager;
        }
    }
}