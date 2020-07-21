namespace Pof
{
    /// <summary>
    /// Represents a potential value of a property
    /// which resulted from processing of a message
    /// </summary>
    public readonly struct Candidate
    {
        /// <summary>
        /// Message signature from the message that delivered the proposed value
        /// </summary>
        public string MessageSignature { get; }
        
        /// <summary>
        /// Proposed value
        /// </summary>
        public object? Value { get; }

        /// <summary>
        /// Creates an instance of the Candidate class
        /// </summary>
        /// <param name="messageSignature">Message signature</param>
        /// <param name="value">Proposed value</param>
        public Candidate(string messageSignature, object? value)
        {
            MessageSignature = messageSignature;
            Value = value;
        }
    }
}