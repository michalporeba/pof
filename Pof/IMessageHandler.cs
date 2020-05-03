using System.Collections.Generic;

namespace Pof
{
    public interface IMessageHandler
    {
        void HandleMessage(Message message);
        bool HasConflicts();
    }
}