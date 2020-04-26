using System;

public class Message
{
    public string Hash { get; }
    public MessageContent Content { get; }

    public class MessageContent
    {
        public string MessageType { get; set; }
        public Guid ObjectId { get; set; }
               
    }
}