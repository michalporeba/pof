namespace Pof
{
    public interface IEntityManager<out TEntity>
        : IMessageHandler
    {
        TEntity Entity { get; }
        string Topic { get; }
        
        void Connect(IMessagePump messagePump);
        void Commit();

    }
}