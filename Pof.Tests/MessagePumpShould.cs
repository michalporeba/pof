using System;
using Moq;
using NUnit.Framework;
using Pof.Data;

namespace Pof.Tests
{
    public class MessagePumpShould
    {
        private readonly IMessageStore _messageStore = new InMemoryMessageStore();
        
        [Test]
        public void store_any_incoming_message_in_a_local_storage()
        {
            var pump = CreateTestPump();
            var topic = Guid.NewGuid().ToString();
            var message = new Message();
            pump.Push(topic, message);

            Assert.That(_messageStore.Contains(topic, message));
        }

        [Test]
        public void distribute_messages_to_local_subscribers()
        {
            var pump = CreateTestPump();
            var topic = Guid.NewGuid().ToString();
            var manager1 = EntityManagerFactory.Create(new TestEntity(), pump, topic);
            var manager2 = EntityManagerFactory.Create(new TestEntity(), pump, topic);
            var manager3 = EntityManagerFactory.Create(new TestEntity(), pump, topic);
            var testValue = Guid.NewGuid().ToString();

            Assert.That(manager2.Entity.Name, Is.Not.EqualTo(testValue));
            Assert.That(manager3.Entity.Name, Is.Not.EqualTo(testValue));
            
            manager1.Entity.Name = testValue;
            manager1.Commit();
            Assert.That(manager2.Entity.Name, Is.EqualTo(testValue), nameof(manager2));
            Assert.That(manager3.Entity.Name, Is.EqualTo(testValue), nameof(manager3));
        }
        
        
        private IMessagePump CreateTestPump()
            => new MessagePump(_messageStore);
        
        private class TestEntity
        {
            public string Name { get; set; }
        }
    }
}