using System;
using Moq;
using NUnit.Framework;
using Pof.Data;

namespace Pof.Tests
{
    public class MessagePumpShould
    {
        private Mock<IMessageStore> _messageStore = new Mock<IMessageStore>();
        private string _testTopic = string.Empty;

        [SetUp]
        public void SetUp()
        {
            _messageStore = new Mock<IMessageStore>();
            _testTopic = Guid.NewGuid().ToString().Substring(0, 8);
        }
        
        [Test, Ignore("WIP")]
        public void store_any_incoming_message_in_a_local_storage()
        {
            var pump = CreateTestPump();
            pump.Push(_testTopic, new Message());
        }

        private IMessagePump CreateTestPump()
        {
            return new MessagePump(_messageStore.Object);
        }
    }
}