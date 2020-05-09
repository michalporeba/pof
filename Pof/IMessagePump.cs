namespace Pof
{
    /// <summary>
    /// Represents object capable of distributing messages
    /// </summary>
    public interface IMessagePump
    {
        /// <summary>
        /// Connect to a remote message pump
        /// </summary>
        /// <param name="messagePumpClient"></param>
        void Connect(IMessagePumpClient messagePumpClient);
        
        /// <summary>
        /// Connect local message handler to the pump
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="handler"></param>
        void Connect(string topic, IMessageHandler handler);
        
        /// <summary>
        /// Push a message to the pump and distribute it to all subscribers
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="message"></param>
        void Push(string topic, Message message);
    }
}