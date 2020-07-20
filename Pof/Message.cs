using System;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using System.Collections.Immutable;
using System.Linq;
using Pof.Internal;

namespace Pof
{
    public readonly struct Message
        : IEquatable<Message>
    {
        private readonly MessageContent _content;

        private string _hash { get; }

        public Message(string propertyName, object? value)
            : this(propertyName, new[] { string.Empty }, value)
        {
        }

        public Message(string propertyName, Message predecessor, object? value)
            : this(propertyName, new[] {predecessor.GetHash()}, value)
        {
        }
        
        public Message(string propertyName, string predecessor, object? value)
            : this(propertyName, new[] {predecessor}, value)
        {
        }
        
        public Message(string propertyName, Message[] predecessors, object? value)
        {
            var parentHashes = predecessors.Select(x => x.GetHash()).ToArray();
            _content = new MessageContent(propertyName, parentHashes, value);
            _hash = CalculateHash(_content);
        }
        
        public Message(string propertyName, string[] predecessors, object? value)
        {
            _content = new MessageContent(propertyName, predecessors, value);
            _hash = CalculateHash(_content);
        }

        public void ApplyWith(IMessageHandler writer)
        {
            writer.RouteMessage(this, _content.PropertyName);
        }
        
        public void ApplyWith(IPropertySetter setter)
        {
            setter.AddState(_content.Value, _hash, _content.Predecessors);
        }

        public string GetHash() => _hash;
        
        public override string ToString() => $"Property: {_content.PropertyName}; Value: {_content.Value}; Hash: {_hash};";

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

        public bool Equals(Message other)
        {
            return _hash.Equals(other._hash);
        }

        public override bool Equals(object? obj)
        {
            return obj is Message other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return _hash.GetHashCode();
            }
        }
    }
}