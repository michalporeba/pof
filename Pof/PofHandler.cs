using System.Collections.Generic;

namespace Pof
{
    public class PofHandler<TEntity>
        where TEntity : IPofEntity, new()
    {
        private readonly TEntity _original = new TEntity();
        public TEntity Entity { get; } = new TEntity();
        public void Handle(Message message)
        {
            var property = Entity.GetType().GetProperty(message.PropertyName);
            property.SetValue(Entity, message.Value);
        }

        public IEnumerable<Message> GetNewMessages()
        {
            foreach(var property in Entity.GetType().GetProperties())
            {
                var originalValue = property.GetValue(_original);
                var newValue = property.GetValue(Entity);

                if (originalValue == null && newValue != null
                    || originalValue != null && !originalValue.Equals(newValue)
                )
                {
                    yield return new Message(property.Name, newValue);
                }
            }
        }
    }
}
