using System.Collections.Generic;

namespace Pof.Tests.TestData
{
    public class TestData
    {
        public static IEnumerable<int> SmallIntegers => new[] {-3, 0, 17};
        public static IEnumerable<string> ShortStrings => new[] {"hello", "你好", "こんにちは", "привет"};
        public static IEnumerable<string> BlankOrEmptyStrings => new[] { " ", "" };
    }
}