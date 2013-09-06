using System;
/**/
    using NUnit.Framework;
/**/
using System.Collections.Generic;
using System.Collections;
using DcError = MetaphysicsIndustries.Giza.DefinitionChecker.DcError;


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
            var errors = new List<Error>();
            Grammar grammar = ss.GetGrammar(input, errors);

            Assert.IsEmpty(errors);

            DefinitionChecker dc = new DefinitionChecker();
            errors = new List<Error>(dc.CheckDefinitions(grammar.Definitions));

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

        [Test]
        public void TestNoPathFromStart()
        {
            // setup
            var a = new Definition {
                Name="a"
            };

            var n1 = new CharNode('1');
            var n2 = new CharNode('2');
            var n3 = new CharNode('3');

            a.Nodes.Add(n1);
            a.Nodes.Add(n2);
            a.Nodes.Add(n3);

            a.StartNodes.Add(n1);
            a.EndNodes.Add(n3);
            n1.NextNodes.Add(n3);
            n2.NextNodes.Add(n3);

            var dc = new DefinitionChecker();

            // action
            var errorEnu = dc.CheckDefinition(a);
            var errors = new List<Error>(errorEnu);

            // assertions
            Assert.IsNotNull(errorEnu);
            Assert.AreEqual(1, errors.Count);
            Assert.IsInstanceOf<DcError>(errors[0]);
            var err = (DcError)errors[0];
            Assert.AreEqual(DcError.NodeHasNoPathFromStart, err.ErrorType);
            Assert.AreSame(n2, err.Node);
        }

        [Test]
        public void TestNoPathToEnd()
        {
            // setup
            var a = new Definition {
                Name="a"
            };

            var n1 = new CharNode('1');
            var n2 = new CharNode('2');
            var n3 = new CharNode('3');

            a.Nodes.Add(n1);
            a.Nodes.Add(n2);
            a.Nodes.Add(n3);

            a.StartNodes.Add(n1);
            a.EndNodes.Add(n3);
            n1.NextNodes.Add(n3);
            n1.NextNodes.Add(n2);

            var dc = new DefinitionChecker();

            // action
            var errorEnu = dc.CheckDefinition(a);
            var errors = new List<Error>(errorEnu);

            // assertions
            Assert.IsNotNull(errorEnu);
            Assert.AreEqual(1, errors.Count);
            Assert.IsInstanceOf<DcError>(errors[0]);
            var err = (DcError)errors[0];
            Assert.AreEqual(DcError.NodeHasNoPathToEnd, err.ErrorType);
            Assert.AreSame(n2, err.Node);
        }

        [Test]
        public void TestNextNodeOutsideDefinition1()
        {
            // setup
            var a = new Definition() { Name="a" };
            var b = new Definition() { Name="b" };
            var n1 = new CharNode('1');
            var n2 = new CharNode('2');

            a.Nodes.Add(n1);
            a.StartNodes.Add(n1);
            a.EndNodes.Add(n1);
            b.Nodes.Add(n2);
            b.StartNodes.Add(n2);
            b.EndNodes.Add(n2);

            n1.NextNodes.Add(n2);
            var dc = new DefinitionChecker();

            // action
            var errorEnu = dc.CheckDefinition(a);
            var errors = new List<Error>(errorEnu);

            // assertions
            Assert.IsNotNull(errorEnu);
            Assert.AreEqual(1, errors.Count);
            Assert.IsInstanceOf<DcError>(errors[0]);
            var err = (DcError)errors[0];
            Assert.AreEqual(DcError.NextNodeLinksOutsideOfDefinition, err.ErrorType);
            Assert.AreSame(n1, err.Node);
        }

        [Test]
        public void TestNextNodeOutsideDefinition2()
        {
            // setup
            var a = new Definition() { Name="a" };
            var n1 = new CharNode('1');
            var n2 = new CharNode('2');

            a.Nodes.Add(n1);
            a.StartNodes.Add(n1);
            a.EndNodes.Add(n1);

            n1.NextNodes.Add(n2);
            var dc = new DefinitionChecker();

            // action
            var errorEnu = dc.CheckDefinition(a);
            var errors = new List<Error>(errorEnu);

            // assertions
            Assert.IsNotNull(errorEnu);
            Assert.AreEqual(1, errors.Count);
            Assert.IsInstanceOf<DcError>(errors[0]);
            var err = (DcError)errors[0];
            Assert.AreEqual(DcError.NextNodeLinksOutsideOfDefinition, err.ErrorType);
            Assert.AreSame(n1, err.Node);
        }

        [Test]
        public void TestStartNodeHasWrongParentDefinition1()
        {
            // setup
            var a = new Definition() { Name="a" };
            var b = new Definition() { Name="b" };
            var n1 = new CharNode('1');
            var n2 = new CharNode('2');

            a.Nodes.Add(n1);
            a.StartNodes.Add(n1);
            a.EndNodes.Add(n1);
            b.Nodes.Add(n2);
            b.StartNodes.Add(n2);
            b.EndNodes.Add(n2);

            a.StartNodes.Add(n2);

            var dc = new DefinitionChecker();

            // action
            var errorEnu = dc.CheckDefinition(a);
            var errors = new List<Error>(errorEnu);

            // assertions
            Assert.IsNotNull(errorEnu);
            Assert.AreEqual(1, errors.Count);
            Assert.IsInstanceOf<DcError>(errors[0]);
            var err = (DcError)errors[0];
            Assert.AreEqual(DcError.StartNodeHasWrongParentDefinition, err.ErrorType);
            Assert.AreSame(n2, err.Node);
        }

        [Test]
        public void TestStartNodeHasWrongParentDefinition2()
        {
            // setup
            var a = new Definition() { Name="a" };
            var n1 = new CharNode('1');
            var n2 = new CharNode('2');

            a.Nodes.Add(n1);
            a.StartNodes.Add(n1);
            a.EndNodes.Add(n1);

            a.StartNodes.Add(n2);

            var dc = new DefinitionChecker();

            // action
            var errorEnu = dc.CheckDefinition(a);
            var errors = new List<Error>(errorEnu);

            // assertions
            Assert.IsNotNull(errorEnu);
            Assert.AreEqual(1, errors.Count);
            Assert.IsInstanceOf<DcError>(errors[0]);
            var err = (DcError)errors[0];
            Assert.AreEqual(DcError.StartNodeHasWrongParentDefinition, err.ErrorType);
            Assert.AreSame(n2, err.Node);
        }
    }
}

