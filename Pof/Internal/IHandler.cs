namespace Pof.Internal
{
    internal interface IHandler
    {
        void Handle(Message message);
        bool HasConflicts();
    }
}