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
    }
}