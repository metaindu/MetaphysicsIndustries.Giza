using System;
using NUnit.Framework;
using System.Collections.Generic;

namespace MetaphysicsIndustries.Giza.Test
{
    [TestFixture]
    public class StringFormatterTest
    {
        static Dictionary<string, string> emptyParams = new Dictionary<string, string>();

        [Test]
        public void TestNoParams()
        {
            // setup
            var format = "no params";

            // action
            var result = StringFormatter.Format(format, emptyParams);

            // assertions
            Assert.AreEqual(format, result);
        }

        [Test]
        public void TestSingleParam1()
        {
            // setup
            var format = "single {param}";
            var values = new Dictionary<string, string>() { { "param", "result" } };

            // action
            var result = StringFormatter.Format(format, values);

            // assertions
            Assert.AreEqual("single result", result);
        }

        [Test]
        public void TestSingleParam2()
        {
            // setup
            var format = "single { param}";
            var values = new Dictionary<string, string>() { { "param", "result" } };

            // action
            var result = StringFormatter.Format(format, values);

            // assertions
            Assert.AreEqual("single result", result);
        }

        [Test]
        public void TestSingleParam3()
        {
            // setup
            var format = "single {param }";
            var values = new Dictionary<string, string>() { { "param", "result" } };

            // action
            var result = StringFormatter.Format(format, values);

            // assertions
            Assert.AreEqual("single result", result);
        }

        [Test]
        public void TestSingleParam4()
        {
            // setup
            var format = "single { param }";
            var values = new Dictionary<string, string>() { { "param", "result" } };

            // action
            var result = StringFormatter.Format(format, values);

            // assertions
            Assert.AreEqual("single result", result);
        }

        [Test]
        public void TestMissingParam1()
        {
            // setup
            var format = "missing {param}";

            // action
            var result = StringFormatter.Format(format, emptyParams);

            // assertions
            Assert.AreEqual(format, result);
        }

        [Test]
        public void TestMissingParam2()
        {
            // setup
            var format = "missing { param}";

            // action
            var result = StringFormatter.Format(format, emptyParams);

            // assertions
            Assert.AreEqual(format, result);
        }

        [Test]
        public void TestMissingParam3()
        {
            // setup
            var format = "missing {param }";

            // action
            var result = StringFormatter.Format(format, emptyParams);

            // assertions
            Assert.AreEqual(format, result);
        }

        [Test]
        public void TestMissingParam4()
        {
            // setup
            var format = "missing { param }";

            // action
            var result = StringFormatter.Format(format, emptyParams);

            // assertions
            Assert.AreEqual(format, result);
        }

        [Test]
        public void TestMoreValuesThanParams()
        {
            // setup
            var format = "more {values} than params";
            var values = new Dictionary<string, string>() {
                { "param", "result" },
                { "values", "widgets" },
            };

            // action
            var result = StringFormatter.Format(format, values);

            // assertions
            Assert.AreEqual("more widgets than params", result);
        }

        [Test]
        public void TestMoreParamsThanValues()
        {
            // setup
            var format = "more {params} than {values}";
            var values = new Dictionary<string, string>() {
                { "values", "widgets" },
            };

            // action
            var result = StringFormatter.Format(format, values);

            // assertions
            Assert.AreEqual("more {params} than widgets", result);
        }

        [Test]
        public void TestEscape1()
        {
            // setup
            var format = "escape {{";

            // action
            var result = StringFormatter.Format(format, emptyParams);

            // assertions
            Assert.AreEqual("escape {", result);
        }

        [Test]
        public void TestEscape2()
        {
            // setup
            var format = "escape }}";

            // action
            var result = StringFormatter.Format(format, emptyParams);

            // assertions
            Assert.AreEqual("escape }", result);
        }

        [Test]
        public void TestEscape3()
        {
            // setup
            var format = "escapes {{text}}";

            // action
            var result = StringFormatter.Format(format, emptyParams);

            // assertions
            Assert.AreEqual("escapes {text}", result);
        }

        [Test]
        public void TestDifficultEscape()
        {
            // setup
            var format = "difficult {{{escape}}}";
            var values = new Dictionary<string, string>() {
                { "escape", "something" },
            };
            // action
            var result = StringFormatter.Format(format, values);

            // assertions
            Assert.AreEqual("difficult {something}", result);
        }

        [Test]
        public void TestDifficultEscapeWithMissingParam()
        {
            // setup
            var format = "difficult {{{escape}}} with missing param";

            // action
            var result = StringFormatter.Format(format, emptyParams);

            // assertions
            Assert.AreEqual("difficult {{escape}} with missing param", result);
        }
    }
}

