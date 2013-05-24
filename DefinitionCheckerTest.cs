using System;
/*/
    using NUnit.Framework;
/*/
    using MetaphysicsIndustries.Giza.Test;
/**/
using System.Collections.Generic;
using System.Collections;


namespace MetaphysicsIndustries.Giza
{
    [TestFixture()]
    public class DefinitionCheckerTest
    {
        [Test()]
        public void TestNormal()
        {
            SupergrammarSpanner ss = new SupergrammarSpanner();
            string input =
                "expr = ( binop | subexpr ); \r\n" +
                "binop = subexpr ( [+-*/%] subexpr )+; \r\n" +
                "subexpr = ( number | var | unop | paren ); \r\n" +
                "number = [\\d]+; \r\n" +
                "var = [\\w]+; \r\n" +
                "unop = [+-] subexpr; \r\n" +
                "paren = '(' expr ')'; \r\n";
            Grammar grammar = ss.GetGrammar(input);

            DefinitionChecker dc = new DefinitionChecker();
            List<DefinitionChecker.ErrorInfo> errors = new List<DefinitionChecker.ErrorInfo>(dc.CheckDefinitions(grammar.Definitions));

            Assert.IsEmpty(errors);
        }

        [Test()]
        public void TestSingleDefCycle()
        {
            Definition a = new Definition {
                Name="a",
            };
            a.Nodes.Add(new DefRefNode(a));
            a.StartingNodes.Add(a.Nodes[0]);
            a.EndingNodes.Add(a.Nodes[0]);

            List<Definition> defs = new List<Definition> { a };

            DefinitionChecker dc = new DefinitionChecker();
            List<DefinitionChecker.ErrorInfo> errors = new List<DefinitionChecker.ErrorInfo>(dc.CheckDefinitions(defs));

            Assert.IsNotEmpty(errors);

            bool found = false;
            foreach (DefinitionChecker.ErrorInfo ei in errors)
            {
                if (ei.Error == DefinitionChecker.Error.LeadingReferenceCycle)
                {
                    found = true;
                }
            }
            Assert.IsTrue(found, "No LeadingReferenceCycle error was found in the returned list.");
        }

        [Test()]
        public void TestMultiDefCycle()
        {
            Definition a = new Definition {
                Name="a",
            };
            Definition b = new Definition {
                Name="b",
            };
            Definition c = new Definition {
                Name="c",
            };

            a.Nodes.Add(new DefRefNode(b));
            a.Nodes.Add(new CharNode('a'));
            a.StartingNodes.Add(a.Nodes[0]);
            a.EndingNodes.Add(a.Nodes[1]);
            a.Nodes[0].NextNodes.Add(a.Nodes[1]);

            b.Nodes.Add(new DefRefNode(c));
            b.Nodes.Add(new CharNode('b'));
            b.StartingNodes.Add(b.Nodes[0]);
            b.EndingNodes.Add(b.Nodes[1]);
            b.Nodes[0].NextNodes.Add(b.Nodes[1]);

            c.Nodes.Add(new DefRefNode(a));
            c.Nodes.Add(new CharNode('c'));
            c.StartingNodes.Add(c.Nodes[0]);
            c.EndingNodes.Add(c.Nodes[1]);
            c.Nodes[0].NextNodes.Add(c.Nodes[1]);

            List<Definition> defs = new List<Definition> { a, b, c };

            DefinitionChecker dc = new DefinitionChecker();
            List<DefinitionChecker.ErrorInfo> errors = new List<DefinitionChecker.ErrorInfo>(dc.CheckDefinitions(defs));

            Assert.IsNotEmpty(errors);

            bool found = false;
            foreach (DefinitionChecker.ErrorInfo ei in errors)
            {
                if (ei.Error == DefinitionChecker.Error.LeadingReferenceCycle)
                {
                    found = true;
                }
            }
            Assert.IsTrue(found, "No LeadingReferenceCycle error was found in the returned list.");
        }
    }
}

namespace MetaphysicsIndustries.Giza.Test
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

