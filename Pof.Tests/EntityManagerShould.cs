using System;
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

        [Test]
        public void update_string_property_from_message([ShortStrings]string value)
        {
            var manager = CreateTestEntityManager();
            manager.HandleMessage(new Message(nameof(TestEntity.StringProperty), value));
            Assert.That(manager.Entity.StringProperty, Is.EqualTo(value));
        }

        [Test]
        public void update_int_property_from_message([SmallIntegers]int value)
        {
            var manager = CreateTestEntityManager();
            manager.HandleMessage(new Message(nameof(TestEntity.IntegerProperty), value));
            Assert.That(manager.Entity.IntegerProperty, Is.EqualTo(value));
        }

        [Test]
        public void update_multiple_properties_from_a_collection_of_messages(
            [ShortStrings]string stringValue,
            [SmallIntegers]int integerValue)
        {
            var manager = CreateTestEntityManager();
            manager.HandleMessage(new Message(nameof(TestEntity.IntegerProperty), integerValue));
            manager.HandleMessage(new Message(nameof(TestEntity.StringProperty), stringValue));

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
            manager.HandleMessage(new Message(nameof(TestEntity.StringProperty), "a"));
            manager.HandleMessage(new Message(nameof(TestEntity.StringProperty), "b"));

            Assert.That(manager.HasConflicts(), Is.True, "There should be a conflict");
            Assert.That(manager.Entity.StringProperty, Is.EqualTo("b"), "Last value should be 'b'");
        }

        [Test]
        public void set_property_value_to_the_value_from_the_last_message()
        {
            var manager = CreateTestEntityManager();
            var message1 = new Message(nameof(TestEntity.StringProperty), "a");    
            var message2 = new Message(nameof(TestEntity.StringProperty), message1.Hash, "b");
            var message3 = new Message(nameof(TestEntity.StringProperty), message2.Hash, "c");
            
            manager.HandleMessage(message1);
            manager.HandleMessage(message2);
            manager.HandleMessage(message3);
            
            Assert.That(manager.HasConflicts, Is.False, "There should be no conflicts");
            Assert.That(manager.Entity.StringProperty, Is.EqualTo("c"), "Current value should be 'c'");
        }
        
        [Test]
        public void set_property_value_to_the_value_from_the_last_message_regardless_of_order_of_processing()
        {
            var manager = CreateTestEntityManager();
            var message1 = new Message(nameof(TestEntity.StringProperty), "a");    
            var message2 = new Message(nameof(TestEntity.StringProperty), message1.Hash, "b");
            var message3 = new Message(nameof(TestEntity.StringProperty), message2.Hash, "c");
            
            manager.HandleMessage(message3);
            manager.HandleMessage(message2);
            manager.HandleMessage(message1);
            
            Assert.That(manager.HasConflicts, Is.False, "There should be no conflicts");
            Assert.That(manager.Entity.StringProperty, Is.EqualTo("c"), "Current value should be 'c'");
        }

        [Test]
        public void resolve_conflicts()
        {
            var manager = CreateTestEntityManager();
            var message1 = new Message(nameof(TestEntity.StringProperty), "a");    
            var message2 = new Message(nameof(TestEntity.StringProperty), message1.Hash, "b");
            var message3 = new Message(nameof(TestEntity.StringProperty), message1.Hash, "c");
            
            manager.HandleMessage(message1);
            manager.HandleMessage(message2);
            manager.HandleMessage(message3);
            
            Assume.That(manager.HasConflicts(), Is.True, "There should be conflict in the setup");
            
            var message4 = new Message(nameof(TestEntity.StringProperty), new [] { message2.Hash, message3.Hash }, "d");
            manager.HandleMessage(message4);
            
            Assert.That(manager.HasConflicts, Is.False, "There should be no conflicts any more");
            Assert.That(manager.Entity.StringProperty, Is.EqualTo("d"), "Current value should be 'd'");
        }
        
        #region "Pump Interactions"

        [Test]
        public void subscribe_to_pump_on_creation()
        {
            var manager = CreateTestEntityManager();
            var topic = _testEntity.Id.ToString();
            _pump.Verify(x => x.Connect(topic, manager), Times.Once);
        }
        
        #endregion    
        
        private static void SetProperty(object target, string property, object value)
        {
            target.GetType().GetProperty(property)?.SetValue(target, value);
        }

        private IEntityManager<TestEntity> CreateTestEntityManager()
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