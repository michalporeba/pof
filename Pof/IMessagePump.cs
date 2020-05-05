namespace Pof
{
    public interface IMessagePump
    {
        void Connect(string topic, IMessageHandler handler);
    }
}