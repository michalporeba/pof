namespace Pof
{
    public interface IMessagePump
    {
        void Connect(IMessagePumpClient messagePumpClient);
        void Connect(string topic, IMessageHandler handler);
        void Push(string topic, Message message);
    }
}