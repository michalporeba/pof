using System;

namespace Pof
{
    public class PofHandler<TEntity>
        where TEntity : IPofEntity, new()
    {
        public TEntity Entity { get; } = new TEntity();
        public void Handle(Message message)
        {
            var property = Entity.GetType().GetProperty(message.PropertyName);
            property.SetValue(Entity, message.Value);
        }
    }
}
