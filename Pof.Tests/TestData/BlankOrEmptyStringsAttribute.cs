using NUnit.Framework;

namespace Pof.Tests.TestData
{
    internal class BlankOrEmptyStringsAttribute : ValueSourceAttribute
    {
        public BlankOrEmptyStringsAttribute()
            : base(typeof(TestData), nameof(TestData.BlankOrEmptyStrings))
        {
        }
    }
}