using System;

namespace Pof
{
    /// <summary>
    /// An object capable of handling incoming messages
    /// </summary>
    public interface IMessageHandler
    {
        /// <summary>
        /// Gets ID of the message handler
        /// which can be used to manage subscriptions
        /// </summary>
        Guid Id { get; }
        
        /// <summary>
        /// Handles an incoming message
        /// </summary>
        /// <param name="message">The message</param>
        void HandleMessage(Message message);
        
        /// <summary>
        /// Checks if there are any conflicts in the manage entity
        /// </summary>
        /// <returns>Returns true if there is at least one conflict</returns>
        bool HasConflicts();
    }
}