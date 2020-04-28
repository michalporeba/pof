using NUnit.Framework;

namespace Pof.Tests.TestData
{
    public class ShortStringsAttribute : ValueSourceAttribute
    {
        public ShortStringsAttribute()
            : base(typeof(TestData), nameof(TestData.ShortStrings))
        {
            
        }
    }
}