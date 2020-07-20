using System;
using Moq;
using NUnit.Framework;

namespace Pof.Tests
{
    public class MessageDispatcherShould
    {
        [Test]
        public void not_do_anything_if_there_are_no_subscribers()
        {
            var topic = Guid.NewGuid().ToString();
            var dispatcher = new MessageDispatcher();
            dispatcher.Dispatch(topic, CreateTestMessage());
            Assert.Pass(); // if we got, here, no exception was thrown.
        }

        [Test]
        public void send_message_to_all_subscribers_of_a_topic()
        {
            var topic = Guid.NewGuid().ToString();
            var dispatcher = new MessageDispatcher();
            var handler1Id = Guid.NewGuid();
            var handler1 = new Mock<IMessageHandler>();
            handler1.SetupGet(x => x.Id).Returns(handler1Id);
            handler1.Setup(x => x.RouteMessage(It.IsAny<Message>(), It.IsAny<string>()));
            var handler2Id = Guid.NewGuid();
            var handler2 = new Mock<IMessageHandler>();
            handler2.SetupGet(x => x.Id).Returns(handler2Id);
            handler2.Setup(x => x.RouteMessage(It.IsAny<Message>(), It.IsAny<string>()));

            dispatcher.Subscribe(handler1.Object, topic);
            dispatcher.Subscribe(handler2.Object, topic);
            dispatcher.Dispatch(topic, CreateTestMessage());
            
            handler1.Verify(x => x.RouteMessage(It.IsAny<Message>(), It.IsAny<string>()), Times.Once);
            handler2.Verify(x => x.RouteMessage(It.IsAny<Message>(), It.IsAny<string>()), Times.Once);
        }
        
        [Test]
        public void send_message_to_subscribers_of_the_specific_topic_only()
        {
            var topic1 = Guid.NewGuid().ToString();
            var topic2 = Guid.NewGuid().ToString();
            var dispatcher = new MessageDispatcher();
            var handler1Id = Guid.NewGuid();
            var handler1 = new Mock<IMessageHandler>();
            handler1.SetupGet(x => x.Id).Returns(handler1Id);
            handler1.Setup(x => x.RouteMessage(It.IsAny<Message>(), It.IsAny<string>()));
            var handler2Id = Guid.NewGuid();
            var handler2 = new Mock<IMessageHandler>();
            handler2.SetupGet(x => x.Id).Returns(handler2Id);
            handler2.Setup(x => x.RouteMessage(It.IsAny<Message>(), It.IsAny<string>()));

            dispatcher.Subscribe(handler1.Object, topic1);
            dispatcher.Subscribe(handler2.Object, topic2);
            dispatcher.Dispatch(topic1, CreateTestMessage());
            
            handler1.Verify(x => x.RouteMessage(It.IsAny<Message>(), It.IsAny<string>()), Times.Once);
            handler2.Verify(x => x.RouteMessage(It.IsAny<Message>(), It.IsAny<string>()), Times.Never);
        }
        
        private Message CreateTestMessage()
        {
            var propertyName = Guid.NewGuid().ToString();
            var propertyValue = Guid.NewGuid();
            return new Message(propertyName, propertyValue);
        }

        private class TestEntity
        {
        }
    }
}