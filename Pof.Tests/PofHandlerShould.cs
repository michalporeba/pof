using System;
using System.Linq;
using NUnit.Framework;

namespace Pof.Tests
{
    public class PofEntityShould
    {
        private PofHandler<TestEntity> _handler;

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

        [TestCase("abc")]
        [TestCase("cba")]
        public void update_string_property_from_message(string value)
        {
            _handler.Handle(new Message(nameof(TestEntity.StringProperty), value));
            Assert.That(_handler.Entity.StringProperty, Is.EqualTo(value));
        }

        [TestCase(11)]
        [TestCase(145)]
        public void update_string_property_from_message(int value)
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
            _handler.Entity.StringProperty = value.ToString();
            var message = _handler.GetNewMessages().FirstOrDefault();
            Assert.That(message.PropertyName, Is.EqualTo(propertyName), "Wrong property name");
            Assert.That(message.Value, Is.EqualTo(value), "Value is incorrect");
        }

        public class TestEntity : IPofEntity
        {
            public Guid Id { get; } = Guid.Empty;
            public string StringProperty { get; set; }
            public int IntegerProperty { get; set; }
        }
    }
}