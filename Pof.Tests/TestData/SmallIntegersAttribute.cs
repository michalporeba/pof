using NUnit.Framework;

namespace Pof.Tests.TestData
{
    public class SmallIntegersAttribute : ValueSourceAttribute
    {
        public SmallIntegersAttribute()
            : base(typeof(TestData), nameof(TestData.SmallIntegers))
        {
        }
    }
}