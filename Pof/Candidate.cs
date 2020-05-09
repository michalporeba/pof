namespace Pof
{
    /// <summary>
    /// Represents a potential value of a property
    /// which resulted from processing of a message
    /// </summary>
    public class Candidate
    {
        /// <summary>
        /// Message hash from the message that delivered the proposed value
        /// </summary>
        public string MessageHash { get; }
        
        /// <summary>
        /// Proposed value
        /// </summary>
        public object? Value { get; }

        /// <summary>
        /// Creates an instance of the Candidate class
        /// </summary>
        /// <param name="messageHash">Message hash</param>
        /// <param name="value">Proposed value</param>
        public Candidate(string messageHash, object? value)
        {
            MessageHash = messageHash;
            Value = value;
        }
    }
}