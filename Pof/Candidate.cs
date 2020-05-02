namespace Pof
{
    public class Candidate
    {
        public string MessageHash { get; }
        public object? Value { get; }

        public Candidate(string messageHash, object? value)
        {
            MessageHash = messageHash;
            Value = value;
        }
    }
}