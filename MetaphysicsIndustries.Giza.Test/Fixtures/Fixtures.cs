using System;
using System.Collections;


namespace MetaphysicsIndustries.Giza.Test.Fixtures
{
    public class TestFixtureAttribute : Attribute
    {
    }

    public class TestAttribute : Attribute
    {
    }

    public static class Assert
    {
        public static void IsEmpty(IEnumerable collection)
        {
        }

        public static void IsNotEmpty(IEnumerable collection)
        {
        }

        public static void IsTrue(bool condition, string message)
        {
        }
    }
}
