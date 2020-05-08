using System.Collections.Immutable;

namespace Pof.Data
{
    public interface IMessageStore
    {
        void Save(string topic, Message message);
        IImmutableList<Message> GetAllFromTopic(string topic);
    }
}