using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Pof.Tests.TestData;

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
            [ValueSource(nameof(ObjectsWithNoObviousId))]
            [ValueSource(nameof(ObjectsWithRegularId))]
            object entity
        )
        {
            var topic = Guid.NewGuid().ToString().Substring(0, 4);
            var manager = EntityManagerFactory.Create(entity, _pump.Object, topic);
            Assert.That(manager.Topic, Is.EqualTo(topic), $"{nameof(manager.Topic)} should be {topic}");
            _pump.Verify(x => x.Connect(topic, manager), $"{topic} should be passed to pump as topic");
        }
        
        [Test]
        public void use_the_id_as_topic_by_default(
            [ValueSource(nameof(ObjectsWithRegularId))]
            object entity
        )
        {
            var topic = entity.GetType().GetProperty("Id")?.GetValue(entity)?.ToString() ?? string.Empty; 
            var manager = EntityManagerFactory.Create(entity, _pump.Object);
            Assert.That(manager.Topic, Is.EqualTo(topic), $"{nameof(manager.Topic)} should be {topic}");
            _pump.Verify(x => x.Connect(topic, manager), $"{topic} should be passed to pump as topic");
        }
        
        [Test]
        public void use_custom_property_for_topic_if_requested(
            [ValueSource(nameof(ObjectsWithAlternativeId))]
            object entity
        )
        {
            const string idPropertyName = nameof(AnotherKeyEntity<object>.AlternativeId);
            var topic = entity.GetType().GetProperty(idPropertyName)?.GetValue(entity)?.ToString() ?? string.Empty; 
            var manager = EntityManagerFactory.Create(entity, idPropertyName, _pump.Object);
            Assert.That(manager.Topic, Is.EqualTo(topic), $"{nameof(manager.Topic)} should be {topic}");
            _pump.Verify(x => x.Connect(topic, manager), $"{topic} should be passed to pump as topic");
        }

        [Test]
        public void throw_if_topic_is_blank_or_empty([BlankOrEmptyStrings]string topic)
        {
            var entity = new RegularIdEntity<Guid>();
            
            Assert.That(() =>
            {
                EntityManagerFactory.Create(entity, _pump.Object, topic);
            }, Throws.InstanceOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void throw_if_custom_id_property_doesnt_exist([ShortStrings] string value)
        {
            var entity = new NoIdEntity();

            Assert.That(() =>
            {
                EntityManagerFactory.Create(entity, value, _pump.Object);
            }, Throws.InstanceOf<ArgumentOutOfRangeException>());
        }
        // TODO: throw if custom property doesn't exist
        
        private static IEnumerable<object> ObjectsWithNoObviousId => new[]
        {
            (object)new NoIdEntity(),
            new AnotherKeyEntity<Guid>(),
            new AnotherKeyEntity<int>(),
            new AnotherKeyEntity<string>()
        };

        private static IEnumerable<object> ObjectsWithRegularId => new[]
        {
            (object)new RegularIdEntity<Guid> { Id = Guid.NewGuid() },
            new RegularIdEntity<int> { Id = 17 },
            new RegularIdEntity<string> { Id = "hello" }
        };
        
        private static IEnumerable<object> ObjectsWithAlternativeId => new[]
        {
            (object)new AnotherKeyEntity<Guid> { AlternativeId = Guid.NewGuid() },
            new AnotherKeyEntity<int> { AlternativeId = 23 }, 
            new AnotherKeyEntity<string> { AlternativeId = "abc123" }
        };

        private class NoIdEntity
        {
        }

        private class RegularIdEntity<T> where T : notnull
        {
            // ReSharper disable once UnusedAutoPropertyAccessor.Local
            public T Id { get; set; } = default!;
        }

        private class AnotherKeyEntity<T>
        {
            public T AlternativeId { get; set; } = default!;
        }
    }
}