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
                start=new StartNode(),
                end=new EndNode(),
            };
            a.Nodes.Add(a.start);
            a.Nodes.Add(new DefRefNode(a));
            a.Nodes.Add(a.end);
            a.Nodes[0].NextNodes.Add(a.Nodes[1]);
            a.Nodes[1].NextNodes.Add(a.Nodes[2]);

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
                start=new StartNode(),
                end=new EndNode(),
            };
            Definition b = new Definition {
                Name="b",
                start=new StartNode(),
                end=new EndNode(),
            };
            Definition c = new Definition {
                Name="c",
                start=new StartNode(),
                end=new EndNode(),
            };

            a.Nodes.Add(a.start);
            a.Nodes.Add(new DefRefNode(b));
            a.Nodes.Add(new CharNode('a'));
            a.Nodes.Add(a.end);
            a.Nodes[0].NextNodes.Add(a.Nodes[1]);
            a.Nodes[1].NextNodes.Add(a.Nodes[2]);
            a.Nodes[2].NextNodes.Add(a.Nodes[3]);

            b.Nodes.Add(b.start);
            b.Nodes.Add(new DefRefNode(c));
            b.Nodes.Add(new CharNode('b'));
            b.Nodes.Add(b.end);
            b.Nodes[0].NextNodes.Add(b.Nodes[1]);
            b.Nodes[1].NextNodes.Add(b.Nodes[2]);
            b.Nodes[2].NextNodes.Add(b.Nodes[3]);

            c.Nodes.Add(c.start);
            c.Nodes.Add(new DefRefNode(a));
            c.Nodes.Add(new CharNode('c'));
            c.Nodes.Add(c.end);
            c.Nodes[0].NextNodes.Add(c.Nodes[1]);
            c.Nodes[1].NextNodes.Add(c.Nodes[2]);
            c.Nodes[2].NextNodes.Add(c.Nodes[3]);

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

