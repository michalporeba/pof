using System;
using Moq;
using NUnit.Framework;
using Pof.Data;

namespace Pof.Tests
{
    public class MessagePumpShould
    {
        private readonly IMessageStore _messageStore = new InMemoryMessageStore();

        [SetUp]
        public void SetUp()
        {
            
        }
        
        [Test, Ignore("WIP")]
        public void store_any_incoming_message_in_a_local_storage()
        {
            var pump = CreateTestPump();
            var topic = Guid.NewGuid().ToString();
            var message = new Message();
            pump.Push(topic, message);
            
            //_messageStore.
        }

        private IMessagePump CreateTestPump()
        {
            return new MessagePump(_messageStore);
        }
    }
}