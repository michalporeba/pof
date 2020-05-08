using System;
using System.Linq;
using NUnit.Framework;
using Pof.Data;

namespace Pof.Tests.Data
{
    public class InMemoryMessageStoreShould
    {
        private readonly IMessageStore _store = new InMemoryMessageStore();
        
        [Test]
        public void retrieve_stored_messages_by_topic()
        {
            var topic = Guid.NewGuid().ToString();
            var message = CreateTestMessage();
            _store.Save(topic, message);
            var copy = _store.GetAllFromTopic(topic).First();
            Assert.That(copy.Hash, Is.EqualTo(message.Hash));
        }

        [Test]
        public void be_idempotent()
        {
            var topic = Guid.NewGuid().ToString();
            var message = CreateTestMessage();
            _store.Save(topic, message);
            _store.Save(topic, message);
            _store.Save(topic, message);
            var topicMessages = _store.GetAllFromTopic(topic);
            Assert.That(topicMessages.Count, Is.EqualTo(1));
        }

        [Test]
        public void not_mix_up_topics()
        {
            var topic1 = Guid.NewGuid().ToString();
            var topic2 = Guid.NewGuid().ToString();
            var message1 = CreateTestMessage();
            var message2 = CreateTestMessage();
            var message3 = CreateTestMessage();
            
            _store.Save(topic1, message1);
            _store.Save(topic1, message2);
            _store.Save(topic2, message3);

            var topic1Messages = _store.GetAllFromTopic(topic1);
            var topic2Messages = _store.GetAllFromTopic(topic2);

            Assert.That(topic1Messages.Count, Is.EqualTo(2), "There should be exactly 2 messages in topic 1");
            Assert.That(topic1Messages.Count, Is.EqualTo(2), "There should be exactly 1 message in topic 2");

            Assert.That(topic1Messages.Any(m => m.Hash == message1.Hash), Is.True, "Message 1 should be in topic 1");
            Assert.That(topic1Messages.Any(m => m.Hash == message2.Hash), Is.True, "Message 2 should be in topic 1");
            Assert.That(topic2Messages.Any(m => m.Hash == message3.Hash), Is.True, "Message 3 should be in topic 2");
        }

        [Test]
        public void return_empty_list_from_non_existing_topic()
        {
            var topic = Guid.NewGuid().ToString();
            var messages = _store.GetAllFromTopic(topic);
            Assert.That(messages, Is.Empty);
        }

        [Test]
        public void check_if_message_exists()
        {
            var topic = Guid.NewGuid().ToString();
            var message = CreateTestMessage();

            Assert.That(_store.Contains(topic, message), Is.False, "Should not exist before saving");
            _store.Save(topic, message);
            Assert.That(_store.Contains(topic, message), Is.True, "Should exist once it was saved");
        }

        private Message CreateTestMessage()
        {
            var propertyName = Guid.NewGuid().ToString().Substring(0, 8);
            var propertyValue = Guid.NewGuid();
            return new Message(propertyName, propertyValue);
        }
    }
}