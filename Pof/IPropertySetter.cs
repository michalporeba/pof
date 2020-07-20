using System.Collections.Immutable;

namespace Pof
{
    public interface IPropertySetter
    {
        public void AddState(object? value, string hash, ImmutableSortedSet<string> predecessors);
    }
}