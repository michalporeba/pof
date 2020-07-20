using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using Pof.Tests.TestData;

namespace Pof.Tests
{
    public class EntityManagerShould
    {
        private TestEntity _testEntity = new TestEntity();
        private Mock<IMessagePump> _pump = new Mock<IMessagePump>();
        private readonly List<Message> _messagesSentToThePump = new List<Message>();

        [SetUp]
        public void Setup()
        {
            _testEntity = new TestEntity();
            _pump = new Mock<IMessagePump>();
        }

        [Test]
        public void expose_instantiated_entity()
        {
            var manager = CreateTestEntityManager();
            Assert.That(manager.Entity, Is.Not.Null);
        }

        #region "Updating entity from messages"
        
        [Test]
        public void update_string_property_from_message([ShortStrings]string value)
        {
            var manager = CreateTestEntityManager();
            var message = new Message(nameof(TestEntity.StringProperty), value);
            message.ApplyWith(manager);
            Assert.That(manager.Entity.StringProperty, Is.EqualTo(value));
        }

        [Test]
        public void update_int_property_from_message([SmallIntegers]int value)
        {
            var manager = CreateTestEntityManager();
            var message = new Message(nameof(TestEntity.IntegerProperty), value);
            message.ApplyWith(manager);
            Assert.That(manager.Entity.IntegerProperty, Is.EqualTo(value));
        }

        [Test]
        public void update_multiple_properties_from_a_collection_of_messages(
            [ShortStrings]string stringValue,
            [SmallIntegers]int integerValue)
        {
            var manager = CreateTestEntityManager();
            var message1 = new Message(nameof(TestEntity.IntegerProperty), integerValue);
            var message2 = new Message(nameof(TestEntity.StringProperty), stringValue);
            message1.ApplyWith(manager);
            message2.ApplyWith(manager);

            Assert.That(manager.Entity.IntegerProperty, Is.EqualTo(integerValue), "Integer Value");
            Assert.That(manager.Entity.StringProperty, Is.EqualTo(stringValue), "String Value");
        }
        
        [Test]
        public void have_no_conflicts_when_no_messages_have_been_processed()
        {
            var manager = CreateTestEntityManager();
            Assert.That(manager.HasConflicts(), Is.False);
        }

        [Test]
        public void identify_conflict_if_the_same_property_is_initialised_twice()
        {
            var manager = CreateTestEntityManager();
            var message1 = new Message(nameof(TestEntity.StringProperty), "a");
            var message2 = new Message(nameof(TestEntity.StringProperty), "b");
            message1.ApplyWith(manager);
            message2.ApplyWith(manager);

            Assert.That(manager.HasConflicts(), Is.True, "There should be a conflict");
            Assert.That(manager.Entity.StringProperty, Is.EqualTo("b"), "Last value should be 'b'");
        }

        [Test]
        public void set_property_value_to_the_value_from_the_last_message()
        {
            var manager = CreateTestEntityManager();
            var message1 = new Message(nameof(TestEntity.StringProperty), "a");    
            var message2 = new Message(nameof(TestEntity.StringProperty), message1, "b");
            var message3 = new Message(nameof(TestEntity.StringProperty), message2, "c");

            message1.ApplyWith(manager);
            message2.ApplyWith(manager);
            message3.ApplyWith(manager);
            
            Assert.That(manager.HasConflicts, Is.False, "There should be no conflicts");
            Assert.That(manager.Entity.StringProperty, Is.EqualTo("c"), "Current value should be 'c'");
        }
        
        [Test]
        public void set_property_value_to_the_value_from_the_last_message_regardless_of_order_of_processing()
        {
            var manager = CreateTestEntityManager();
            var message1 = new Message(nameof(TestEntity.StringProperty), "a");    
            var message2 = new Message(nameof(TestEntity.StringProperty), message1, "b");
            var message3 = new Message(nameof(TestEntity.StringProperty), message2, "c");

            message3.ApplyWith(manager);
            message2.ApplyWith(manager);
            message1.ApplyWith(manager);
            
            Assert.That(manager.HasConflicts, Is.False, "There should be no conflicts");
            Assert.That(manager.Entity.StringProperty, Is.EqualTo("c"), "Current value should be 'c'");
        }

        [Test]
        public void resolve_conflicts()
        {
            var manager = CreateTestEntityManager();
            var message1 = new Message(nameof(TestEntity.StringProperty), "a");    
            var message2 = new Message(nameof(TestEntity.StringProperty), message1, "b");
            var message3 = new Message(nameof(TestEntity.StringProperty), message1, "c");
            
            message1.ApplyWith(manager);
            message2.ApplyWith(manager);
            message3.ApplyWith(manager);
            
            Assume.That(manager.HasConflicts(), Is.True, "There should be conflict in the setup");
            
            var message4 = new Message(nameof(TestEntity.StringProperty), new [] { message2, message3 }, "d");
            message4.ApplyWith(manager);
            
            Assert.That(manager.HasConflicts, Is.False, "There should be no conflicts any more");
            Assert.That(manager.Entity.StringProperty, Is.EqualTo("d"), "Current value should be 'd'");
        }
        
        #endregion
        
        #region "Emiting messages"

        [Test]
        public void emit_messages_for_all_properties_on_first_commit()
        {
            var manager = CreateTestEntityManager();
            SetCallbackOnPumpFor(manager.Topic);
            manager.Entity.IntegerProperty = 99;
            manager.Entity.StringProperty = "hello there";
            manager.Commit();

            Assert.That(_messagesSentToThePump.Count, Is.EqualTo(2), "There should be exactly 2 messages");

            // TODO: see how this test could be rewritten or simply removed
            
            // var integerMessage = GetPumpMessageFor(nameof(manager.Entity.IntegerProperty));
            // var stringMessage = GetPumpMessageFor(nameof(manager.Entity.StringProperty));

            // Assert.That(integerMessage.Value, Is.EqualTo(manager.Entity.IntegerProperty), nameof(manager.Entity.IntegerProperty));
            // Assert.That(stringMessage.Value, Is.EqualTo(manager.Entity.StringProperty), nameof(manager.Entity.StringProperty));
        }
        
        [Test]
        public void emit_no_messages_for_consecutive_commits_with_no_changes_between()
        {
            var manager = CreateTestEntityManager();
            SetCallbackOnPumpFor(manager.Topic);
            manager.Entity.IntegerProperty = 111;
            manager.Entity.StringProperty = "some text";
            manager.Commit();
            _messagesSentToThePump.Clear();
            manager.Commit();
            
            Assert.That(_messagesSentToThePump.Count, Is.EqualTo(0), "There should be no messages");
        }

        [Test]
        public void emit_messages_for_changes_between_commits()
        {
            var manager = CreateTestEntityManager();
            manager.Commit();

            SetCallbackOnPumpFor(manager.Topic);
            Assert.That(_messagesSentToThePump, Is.Empty);
            
            manager.Entity.IntegerProperty = 17;
            manager.Entity.StringProperty = "later changes";
            manager.Commit();

            Assert.That(_messagesSentToThePump.Count, Is.EqualTo(2), "There should be exactly 2 messages");
            
            // TODO: see how this test can be rewritten, or perhaps simply removed

            //var integerMessage = GetPumpMessageFor(nameof(manager.Entity.IntegerProperty));
            //var stringMessage = GetPumpMessageFor(nameof(manager.Entity.StringProperty));

            // Assert.That(integerMessage.Value, Is.EqualTo(manager.Entity.IntegerProperty), nameof(manager.Entity.IntegerProperty));
            // Assert.That(stringMessage.Value, Is.EqualTo(manager.Entity.StringProperty), nameof(manager.Entity.StringProperty));            
        }
        
        #endregion 
        
        #region "Pump Interactions"

        #endregion

        private void SetCallbackOnPumpFor(string topic)
        {
            _messagesSentToThePump.Clear();
            _pump.Setup(x => x.Push(topic, It.IsAny<Message>()))
                .Callback<string, Message>((t, m) => { _messagesSentToThePump.Add(m); });
        }
        
        private EntityManager<TestEntity> CreateTestEntityManager()
        {
            return EntityManagerFactory.Create(_testEntity, _pump.Object);
        }
        
        private class TestEntity
        {
            public Guid Id { get; } = Guid.NewGuid();
            public string StringProperty { get; set; } = string.Empty;
            public int IntegerProperty { get; set; }
        }
    }
}