using System;

namespace Pof
{
    public class PofHandler<TEntity>
        where TEntity : IPofEntity, new()
    {
        public TEntity Entity { get; } = new TEntity();
    }
}
