using System;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Pof.Tests.TestData;

namespace Pof.Tests
{
    public class MessageShould
    {
        [Test]
        public void set_property_name_from_constructor(
            [ShortStrings] string value
        )
        {
            var test = new Message(value, null);
            Assert.That(test.PropertyName, Is.EqualTo(value));
        }

        [Test]
        public void set_value_from_constructor(
            [ShortStrings] [SmallIntegers] object value
        )
        {
            var test = new Message(string.Empty, value);
            Assert.That(test.Value, Is.EqualTo(value));
        }

        [Test]
        public void two_messages_with_the_same_content_should_have_the_same_hash(
            [ShortStrings]string propertyName,
            [SmallIntegers]int value
            )
        {
            var message1 = new Message(propertyName, value);
            var message2 = new Message(propertyName, value);

            Assert.That(message1.Hash, Is.EqualTo(message2.Hash));
        }

        [Test]
        public void two_messages_with_different_properties_should_have_different_hashes()
        {
            var message1 = new Message(Guid.NewGuid().ToString(), "value");
            var message2 = new Message(Guid.NewGuid().ToString(), "value");

            Assert.That(message1.Hash, Is.Not.EqualTo(message2.Hash));
        }

        [Test]
        public void two_messages_with_different_values_should_have_different_hashes()
        {
            var message1 = new Message("propertyName", Guid.NewGuid().ToString());
            var message2 = new Message("propertyName", Guid.NewGuid().ToString());
            Assert.That(message1.Hash, Is.Not.EqualTo(message2.Hash));
        }

        [Test]
        public void two_messages_with_the_same_parents_should_have_the_same_hash()
        {
            var parent = new Message("propertyName", "oldValue");
            var message1 = new Message("propertyName", parent.Hash, "newValue");
            var message2 = new Message("propertyName", parent.Hash, "newValue");
            Assert.That(message1.Hash, Is.EqualTo(message2.Hash));
        }
        
        [Test]
        public void two_messages_with_different_parents_should_have_different_hashes()
        {
            var message1 = new Message("propertyName", "fakeHash1", "newValue");
            var message2 = new Message("propertyName", "fakeHash2", "newValue");
            Assert.That(message1.Hash, Is.Not.EqualTo(message2.Hash));
        }
    }
}