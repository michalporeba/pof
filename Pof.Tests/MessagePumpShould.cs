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
            var message = new Message("test", "test");
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

        [Test]
        public void send_all_existing_messages_for_a_topic_to_a_new_local_handler()
        {
            var pump = CreateTestPump();
            var topic = Guid.NewGuid().ToString();
            var entity1 = new TestEntity();
            var firstName = entity1.Name;
            var manager1 = EntityManagerFactory.Create(entity1, pump, topic);
            manager1.Commit();
            
            var entity2 = new TestEntity();
            Assume.That(entity2.Name, Is.Not.EqualTo(firstName));

            var manager2 = EntityManagerFactory.Create(entity2, pump, topic);
            Assert.That(entity2.Name, Is.EqualTo(firstName));
        }

        [Test]
        public void pass_message_to_other_connected_pumps()
        {
            var topic = Guid.NewGuid().ToString();
            var message = new Message(Guid.NewGuid().ToString(), Guid.NewGuid());
            var pump = CreateTestPump();
            var remote1 = new Mock<IMessagePumpClient>();
            var remote2 = new Mock<IMessagePumpClient>();
            pump.Connect(remote1.Object);
            pump.Connect(remote2.Object);

            pump.Push(topic, message);
            remote1.Verify(x => x.Push(topic, It.IsAny<Message>()), nameof(remote1));
            remote2.Verify(x => x.Push(topic, It.IsAny<Message>()), nameof(remote2));
        }
        
        private IMessagePump CreateTestPump()
            => new MessagePump(_messageStore);
        
        private class TestEntity
        {
            public string Name { get; set; } = Guid.NewGuid().ToString();
        }
    }
}