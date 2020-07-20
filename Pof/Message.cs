using System;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using System.Collections.Immutable;
using Pof.Internal;

namespace Pof
{
    public readonly struct Message
    {
        private readonly MessageContent _content;

        public string Hash { get; }
        public string PropertyName => _content.PropertyName;

        public Message(string propertyName, object? value)
            : this(propertyName, new[] { string.Empty }, value)
        {
        }

        public Message(string propertyName, string predecessor, object? value)
            : this(propertyName, new[] {predecessor}, value)
        {
        }
        
        public Message(string propertyName, string[] predecessors, object? value)
        {
            _content = new MessageContent(propertyName, predecessors, value);
            Hash = CalculateHash(_content);
        }

        public void ApplyWith(IPropertySetter setter)
        {
            setter.AddState(_content.Value, Hash, _content.Predecessors);
        }
        
        public override string ToString() => $"Property: {_content.PropertyName}; Value: {_content.Value}; Hash: {Hash};";

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
            public ImmutableSortedSet<string> Predecessors { get; }
            public string PropertyName { get; }
            public object? Value { get; }

            public MessageContent(string propertyName, string[] parentHashes, object? value)
            {
                Predecessors = ImmutableSortedSet.Create(parentHashes);
                PropertyName = propertyName;
                Value = value;
            }
        }
    }
}