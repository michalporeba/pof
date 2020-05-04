using NUnit.Framework;

namespace Pof.Tests.TestData
{
    internal class SmallIntegersAttribute : ValueSourceAttribute
    {
        public SmallIntegersAttribute()
            : base(typeof(TestData), nameof(TestData.SmallIntegers))
        {
        }
    }
}