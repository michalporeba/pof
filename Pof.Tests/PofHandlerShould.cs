using System.Linq;
using NUnit.Framework;
using Pof.Tests.TestData;

namespace Pof.Tests
{
    public class PofEntityShould
    {
        private PofHandler<TestEntity> _handler = new PofHandler<TestEntity>();

        [SetUp]
        public void Setup()
        {
            _handler = new PofHandler<TestEntity>();
        }

        [Test]
        public void expose_instantiated_entity()
        {
            Assert.That(_handler.Entity, Is.Not.Null);
        }

        [Test]
        public void update_string_property_from_message([ShortStrings]string value)
        {
            _handler.Handle(new Message(nameof(TestEntity.StringProperty), value));
            Assert.That(_handler.Entity.StringProperty, Is.EqualTo(value));
        }

        [Test]
        public void update_int_property_from_message([SmallIntegers]int value)
        {
            _handler.Handle(new Message(nameof(TestEntity.IntegerProperty), value));
            Assert.That(_handler.Entity.IntegerProperty, Is.EqualTo(value));
        }

        [Test]
        public void update_multiple_properties_from_a_collection_of_messages(
            [ShortStrings]string stringValue,
            [SmallIntegers]int integerValue)
        {
            var messages = new[]
            {
                new Message(nameof(TestEntity.IntegerProperty), integerValue),
                new Message(nameof(TestEntity.StringProperty), stringValue)
            };
            
            _handler.Handle(messages);

            Assert.That(_handler.Entity.IntegerProperty, Is.EqualTo(integerValue), "Integer Value");
            Assert.That(_handler.Entity.StringProperty, Is.EqualTo(stringValue), "String Value");
        }
        
        [Test]
        public void have_no_new_messages_when_no_changes_have_been_made_to_the_entity()
        {
            Assert.That(_handler.GetNewMessages(), Is.Empty);
        }

        [TestCase(nameof(TestEntity.IntegerProperty), 1)]
        [TestCase(nameof(TestEntity.IntegerProperty), 13)]
        [TestCase(nameof(TestEntity.StringProperty), "a")]
        [TestCase(nameof(TestEntity.StringProperty), "b")]
        public void have_message_for_properties_changed_directly(string propertyName, object value)
        {
            SetProperty(_handler.Entity, propertyName, value);
            var message = _handler.GetNewMessages().FirstOrDefault();
            Assert.That(message.PropertyName, Is.EqualTo(propertyName), "Wrong property name");
            Assert.That(message.Value, Is.EqualTo(value), "Value is incorrect");
        }

        [Test]
        public void have_no_conflicts_when_no_messages_have_been_processed()
        {
            Assert.That(_handler.HasConflicts(), Is.False);
        }

        [Test]
        public void identify_conflict_if_the_same_property_is_initialised_twice()
        {
            _handler.Handle(new Message(nameof(TestEntity.StringProperty), "a"));
            _handler.Handle(new Message(nameof(TestEntity.StringProperty), "b"));

            Assert.That(_handler.HasConflicts(), Is.True, "There should be a conflict");
            Assert.That(_handler.Entity.StringProperty, Is.EqualTo("b"), "Last value should be 'b'");
        }

        private static void SetProperty(object target, string property, object value)
        {
            target.GetType().GetProperty(property)?.SetValue(target, value);
        }
        
        private class TestEntity : IPofEntity
        {
            public string StringProperty { get; set; } = string.Empty;
            public int IntegerProperty { get; set; }
        }
    }
}