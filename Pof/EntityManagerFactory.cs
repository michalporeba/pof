namespace Pof
{
    public static class EntityManagerFactory
    {
        public static IEntityManager<TEntity> Create<TEntity>(TEntity entity, IMessagePump messagePump)
        {
            var manager = new EntityManager<TEntity>(entity);
            manager.Connect(messagePump);
            var topic = entity.GetType().GetProperty("Id").GetValue(entity).ToString();
            messagePump.Subscribe(topic, manager);
            return manager;
        }
    }
}