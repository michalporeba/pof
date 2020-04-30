using NUnit.Framework;
using Pof.Tests.TestData;

namespace Pof.Tests
{
    public class CandidateShould
    {
        [Test]
        public void set_message_hash_in_constructor([ShortStrings] string value)
        {
            var test = new Candidate(value, null);
            Assert.That(test.MessageHash, Is.EqualTo(value));
        }

        [Test]
        public void set_object_value_in_constructor(
            [ShortStrings]
            [SmallIntegers]
            object value
        )
        {
            var test = new Candidate(string.Empty, value);
            Assert.That(test.Value, Is.EqualTo(value));
        }
    }
}