namespace Pof
{
    public interface IMessagePumpClient
    {
        void Push(string topic, Message message);
    }
}