using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;

namespace Pof.Tests
{
    public class EntityManagerFactoryShould
    {
        private Mock<IMessagePump> _pump = new Mock<IMessagePump>();

        [SetUp]
        public void SetUp()
        {
            _pump = new Mock<IMessagePump>();
        }

        [Test]
        public void throw_if_entity_has_no_id_and_topic_is_not_provided(
            [ValueSource(nameof(ObjectsWithNoObviousId))]object entity
            )
        {
            Assert.That(() =>
            {
                 EntityManagerFactory.Create(entity, _pump.Object);
            }, Throws.InstanceOf<InvalidOperationException>());
        }

        [Test]
        public void use_specified_topic_instead_of_id_if_provided(
            [ValueSource(nameof(ObjectsWithNoObviousId))]object entity
        )
        {
            var topic = Guid.NewGuid().ToString().Substring(0, 4);
            var manager = EntityManagerFactory.Create(entity, _pump.Object, topic);
            Assert.That(manager.Topic, Is.EqualTo(topic), $"{nameof(manager.Topic)} should be {topic}");
            _pump.Verify(x => x.Subscribe(topic, manager), $"{topic} should be passed to pump as topic");
        }

        private static IEnumerable<object> ObjectsWithNoObviousId => new[]
        {
            (object)new NoIdEntity(),
            new AnotherKeyEntity<Guid>(),
            new AnotherKeyEntity<int>(),
            new AnotherKeyEntity<string>()
        };
        
        private class NoIdEntity
        {
        }

        private class AnotherKeyEntity<T>
        {
            public T AnotherProperty { get; set; }
        }
    }
}