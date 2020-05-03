namespace Pof
{
    public interface IMessagePump
    {
        void Subscribe(string topic, IMessageHandler handler);
    }
}