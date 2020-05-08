using System;

namespace Pof
{
    public interface IMessageHandler
    {
        Guid Id { get; }
        void HandleMessage(Message message);
        bool HasConflicts();
    }
}