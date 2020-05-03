namespace Pof
{
    public interface IEntityManager<out TEntity>
        : IMessageHandler
    {
        void Connect(IMessagePump messagePump);
        TEntity Entity { get; }
        string Topic { get; }
    }
}