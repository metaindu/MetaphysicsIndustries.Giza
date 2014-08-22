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
            var values = new Dictionary<string, string> { { "param", "result" } };

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
            var values = new Dictionary<string, string> { { "param", "result" } };

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
            var values = new Dictionary<string, string> { { "param", "result" } };

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
            var values = new Dictionary<string, string> { { "param", "result" } };

            // action
            var result = StringFormatter.Format(format, values);

            // assertions
            Assert.AreEqual("single result", result);
        }

        [Test]
        public void TestMultipleParams()
        {
            // setup
            var format = "{param1} {param2}";
            var values = new Dictionary<string, string> {
                { "param1", "first" },
                { "param2", "second" },
            };

            // action
            var result = StringFormatter.Format(format, values);

            // assertions
            Assert.AreEqual("first second", result);
        }

        [Test]
        public void TestDuplicateParam()
        {
            // setup
            var format = "a {param} in need is a {param} indeed";
            var values = new Dictionary<string, string> {
                { "param", "friend" },
            };

            // action
            var result = StringFormatter.Format(format, values);

            // assertions
            Assert.AreEqual("a friend in need is a friend indeed", result);
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
            var values = new Dictionary<string, string> {
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
            var values = new Dictionary<string, string> {
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
            var values = new Dictionary<string, string> {
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

        [Test]
        public void TestFuncArg1()
        {
            // setup
            var format = "func {param}";
            Func<string, string> values = (x) => (x == "param" ? "arg" : "object");

            // action
            var result = StringFormatter.Format(format, values);

            // assertions
            Assert.AreEqual("func arg", result);
        }

        [Test]
        public void TestFuncArg2()
        {
            // setup
            var format = "func {param} and {something}";
            Func<string, string> values = (x) => (x == "param" ? "arg" : "object");

            // action
            var result = StringFormatter.Format(format, values);

            // assertions
            Assert.AreEqual("func arg and object", result);
        }

        [Test]
        public void TestFuncArg3()
        {
            // setup
            var format = "one {param} two {something} three {another}";
            Func<string, string> values = (x) => (x == "param" ? "arg" : 
                                 x == "something" ? "object" :
                                 null);

            // action
            var result = StringFormatter.Format(format, values);

            // assertions
            Assert.AreEqual("one arg two object three {another}", result);
        }

        [Test]
        public void TestFuncArgCaseToLower()
        {
            // setup
            var format = "func {param} {PARAM} {PaRaM}";
            Func<string, string> values = (x) => (x.ToLower() == "param" ? "arg" : "object");

            // action
            var result = StringFormatter.Format(format, values);

            // assertions
            Assert.AreEqual("func arg arg arg", result);
        }

        [Test]
        public void TestDictionaryCaseInsensitive()
        {
            // setup
            var format = "func {param} {PARAM} {PaRaM}";
            Dictionary<string, string> values;
            values = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase) {
                { "param", "arg" }
            };

            // action
            var result = StringFormatter.Format(format, values);

            // assertions
            Assert.AreEqual("func arg arg arg", result);
        }

        [Test]
        public void TestExtensionMethodDictionary()
        {
            // setup
            var values = new Dictionary<string, string> {
                { "param", "arg" },
                { "something", "object" },
            };

            // action
            var result = "one {param} two {something} three {another}".Format(values);

            // assertions
            Assert.AreEqual("one arg two object three {another}", result);
        }

        [Test]
        public void TestExtensionMethodFuncArg()
        {
            // setup
            Func<string, string> values = (x) => (x == "param" ? "arg" : 
                                                  x == "something" ? "object" :
                                                  null);

            // action
            var result = "one {param} two {something} three {another}".Format(values);

            // assertions
            Assert.AreEqual("one arg two object three {another}", result);
        }

        [Test]
        public void TestArgNull1()
        {
            Assert.Throws<ArgumentNullException>(() => StringFormatter.Format(null, emptyParams));
        }

        [Test]
        public void TestArgNull2()
        {
            IDictionary<string, string> values = null;
            Assert.Throws<ArgumentNullException>(() => StringFormatter.Format("", values));
        }

        [Test]
        public void TestArgNull3()
        {
            Assert.Throws<ArgumentNullException>(() => StringFormatter.Format(null, (x) => null));
        }

        [Test]
        public void TestArgNull4()
        {
            Func<string,string> values = null;
            Assert.Throws<ArgumentNullException>(() => StringFormatter.Format("", values));
        }
    }
}

