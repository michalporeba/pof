using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace Pof
{
    public readonly struct Message
    {
        private readonly MessageContent _content;

        public string Hash { get; }
        public string PropertyName => _content.PropertyName;
        public object? Value => _content.Value;

        public Message(string propertyName, object? value)
        {
            _content = new MessageContent(propertyName, value);
            Hash = CalculateHash(_content);
        }

        private static string CalculateHash(MessageContent content)
        {
            var json = JsonConvert.SerializeObject(content);
            using var sha = SHA256.Create();
            byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(json));
            var stringBuilder = new StringBuilder();
            
            foreach(var b in bytes)
            {
                stringBuilder.Append(b.ToString("x2"));
            }

            return stringBuilder.ToString();
        }
        
        private readonly struct MessageContent
        {
            public string PropertyName { get; }
            public object? Value { get; }

            public MessageContent(string propertyName, object? value)
            {
                PropertyName = propertyName;
                Value = value;
            }
        }
    }
}