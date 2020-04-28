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
        public void have_no_new_messages_when_no_changes_have_been_made_to_the_entity()
        {
            Assert.That(_handler.GetNewMessages(), Is.Empty);
        }

        [TestCase("StringProperty", "a")]
        [TestCase("StringProperty", "b")]
        public void have_message_for_the_changed_properties(string propertyName, object value)
        {
            SetProperty(_handler.Entity, nameof(TestEntity.StringProperty), value);
            var message = _handler.GetNewMessages().FirstOrDefault();
            Assert.That(message.PropertyName, Is.EqualTo(propertyName), "Wrong property name");
            Assert.That(message.Value, Is.EqualTo(value), "Value is incorrect");
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