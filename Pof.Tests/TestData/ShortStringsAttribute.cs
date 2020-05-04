using NUnit.Framework;

namespace Pof.Tests.TestData
{
    internal class ShortStringsAttribute : ValueSourceAttribute
    {
        public ShortStringsAttribute()
            : base(typeof(TestData), nameof(TestData.ShortStrings))
        {
            
        }
    }
}