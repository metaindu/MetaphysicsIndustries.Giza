using System;
/**/
    using NUnit.Framework;
/**/
using System.Collections.Generic;
using System.Collections;


namespace MetaphysicsIndustries.Giza.Test
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
            string error;
            Grammar grammar = ss.GetGrammar(input, out error);

            Assert.IsNull(error);

            DefinitionChecker dc = new DefinitionChecker();
            var errors = dc.CheckDefinitions(grammar.Definitions);

            Assert.IsEmpty(errors);
        }

        [Test()]
        public void TestSingleDefCycle()
        {
            Definition a = new Definition {
                Name="a",
            };
            a.Nodes.Add(new DefRefNode(a));
            a.StartNodes.Add(a.Nodes[0]);
            a.EndNodes.Add(a.Nodes[0]);

            List<Definition> defs = new List<Definition> { a };

            DefinitionChecker dc = new DefinitionChecker();
            var errors = dc.CheckDefinitions(defs);

            Assert.IsNotEmpty(errors);

            bool found = false;
            foreach (var ei in errors)
            {
                if (ei.ErrorType == DefinitionChecker.DcError.LeadingReferenceCycle)
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
            a.StartNodes.Add(a.Nodes[0]);
            a.EndNodes.Add(a.Nodes[1]);
            a.Nodes[0].NextNodes.Add(a.Nodes[1]);

            b.Nodes.Add(new DefRefNode(c));
            b.Nodes.Add(new CharNode('b'));
            b.StartNodes.Add(b.Nodes[0]);
            b.EndNodes.Add(b.Nodes[1]);
            b.Nodes[0].NextNodes.Add(b.Nodes[1]);

            c.Nodes.Add(new DefRefNode(a));
            c.Nodes.Add(new CharNode('c'));
            c.StartNodes.Add(c.Nodes[0]);
            c.EndNodes.Add(c.Nodes[1]);
            c.Nodes[0].NextNodes.Add(c.Nodes[1]);

            List<Definition> defs = new List<Definition> { a, b, c };

            DefinitionChecker dc = new DefinitionChecker();
            var errors = dc.CheckDefinitions(defs);

            Assert.IsNotEmpty(errors);

            bool found = false;
            foreach (Error ei in errors)
            {
                if (ei.ErrorType == DefinitionChecker.DcError.LeadingReferenceCycle)
                {
                    found = true;
                }
            }
            Assert.IsTrue(found, "No LeadingReferenceCycle error was found in the returned list.");
        }
    }
}

