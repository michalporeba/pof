namespace Pof
{
    public class Message
    {
        private readonly MessageContent _content;

        public string Hash { get; }
        public string PropertyName => _content.PropertyName;
        public object? Value => _content.Value;

        public Message(string propertyName, object? value)
        {
            Hash = string.Empty;
            _content = new MessageContent
            {
                PropertyName = propertyName,
                Value = value
            };
        }

        private class MessageContent
        {
            public string PropertyName { get; set; } = string.Empty;
            public object? Value { get; set; }
        }
    }
}