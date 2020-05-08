using System.Collections.Immutable;

namespace Pof.Data
{
    public interface IMessageStore
    {
        bool Contains(string topic, Message message);
        void Save(string topic, Message message);
        IImmutableList<Message> GetAllFromTopic(string topic);
    }
}