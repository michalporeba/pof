using System.Collections.Generic;

namespace Pof
{
    public interface IMessageHandler
    {
        void Handle(Message message);
        bool HasConflicts();
    }
}