﻿using System;
using NUnit.Framework;
using Pof.Tests.TestData;

namespace Pof.Tests
{
    public class MessageShould
    {
        [Test]
        public void two_messages_with_the_same_content_should_have_the_same_hash(
            [ShortStrings]string propertyName,
            [SmallIntegers]int value
            )
        {
            var message1 = new Message(propertyName, value);
            var message2 = new Message(propertyName, value);

            Assert.That(message1.Equals(message2), Is.True);
        }

        [Test]
        public void two_messages_with_different_properties_should_have_different_hashes()
        {
            var message1 = new Message(Guid.NewGuid().ToString(), "value");
            var message2 = new Message(Guid.NewGuid().ToString(), "value");

            Assert.That(message1.Equals(message2), Is.False);
        }

        [Test]
        public void two_messages_with_different_values_should_have_different_hashes()
        {
            var message1 = new Message("propertyName", Guid.NewGuid().ToString());
            var message2 = new Message("propertyName", Guid.NewGuid().ToString());
            Assert.That(message1.Equals(message2), Is.False);
        }

        [Test]
        public void two_messages_with_the_same_parents_should_have_the_same_hash()
        {
            var parent = new Message("propertyName", "oldValue");
            var message1 = new Message("propertyName", parent.GetHash(), "newValue");
            var message2 = new Message("propertyName", parent.GetHash(), "newValue");
            Assert.That(message1.Equals(message2), Is.True);
        }
        
        [Test]
        public void two_messages_with_different_parents_should_have_different_hashes()
        {
            var message1 = new Message("propertyName", "fakeHash1", "newValue");
            var message2 = new Message("propertyName", "fakeHash2", "newValue");
            Assert.That(message1.Equals(message2), Is.False);
        }

        [Test]
        public void include_specific_information_in_string_representation()
        {
            var propertyName = Guid.NewGuid().ToString().Substring(0, 4);
            var propertyValue = Guid.NewGuid().ToString().Substring(0, 4);
            var message = new Message(propertyName, propertyValue);
            var value = message.ToString();
            Assert.That(value, Contains.Substring(message.GetHash()), $"TShould contain the hash");
            Assert.That(value, Contains.Substring(propertyName), $"Should contain property name");
            Assert.That(value, Contains.Substring(propertyValue), "Should contain value");
        }
    }
}