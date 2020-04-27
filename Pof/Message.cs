using System;

public class Message
{
    private readonly MessageContent _content;

    public string Hash { get; }
    
    public string PropertyName => _content.PropertyName;
    public object Value => _content.Value;

    public Message(string propertyName, object value)
    {
        _content = new MessageContent() 
        {
            PropertyName = propertyName,
            Value = value
        };
    }

    private class MessageContent
    {
        public Guid ObjectId { get; set; } = Guid.Empty;
        public string PropertyName { get; set; }
        public object Value { get; set; }
    }
}