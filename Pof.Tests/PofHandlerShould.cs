using System;
using NUnit.Framework;

namespace Pof.Tests
{
    public class PofEntityShould
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void expose_instantiated_entity()
        {
            var handler = new PofHandler<TestEntity>();
            Assert.That(handler.Entity, Is.Not.Null);
        }

        [TestCase("abc")]
        [TestCase("cba")]
        public void update_property_from_message(string value)
        {
            var handler = new PofHandler<TestEntity>();
            handler.Handle(new Message(nameof(TestEntity.StringProperty), value));
            Assert.That(handler.Entity.StringProperty, Is.EqualTo(value));
        }

        public class TestEntity : IPofEntity
        {
            public Guid Id { get; } = Guid.NewGuid();
            public string StringProperty { get; set; }
        }
    }
}