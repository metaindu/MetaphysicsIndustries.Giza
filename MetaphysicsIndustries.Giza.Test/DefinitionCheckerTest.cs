
// MetaphysicsIndustries.Giza - A Parsing System
// Copyright (C) 2008-2020 Metaphysics Industries, Inc.
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 3 of the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301
// USA

using System;
/**/
    using NUnit.Framework;
/**/
using System.Collections.Generic;
using System.Linq;


namespace MetaphysicsIndustries.Giza.Test
{
    [TestFixture()]
    public class DefinitionCheckerTest
    {
        [Test()]
        public void TestNormal()
        {
            // given
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
            var g = ss.GetGrammar(input, errors);
            var gc = new GrammarCompiler();
            var grammar = gc.BuildGrammar(g);
            DefinitionChecker dc = new DefinitionChecker();

            // precondition
            Assert.IsEmpty(errors);

            // when
            errors = new List<Error>(dc.CheckDefinitions(grammar.Definitions));

            // then
            Assert.IsEmpty(errors);
        }

        [Test()]
        public void TestSingleDefCycle()
        {
            var a = new NDefinition {
                Name="a",
            };
            a.Nodes.Add(new DefRefNode(a));
            a.StartNodes.Add(a.Nodes[0]);
            a.EndNodes.Add(a.Nodes[0]);

            var defs = new List<NDefinition> { a };

            DefinitionChecker dc = new DefinitionChecker();
            var errors = dc.CheckDefinitions(defs).ToList();

            Assert.IsNotEmpty(errors);

            bool found = false;
            foreach (var ei in errors)
            {
                if (ei.ErrorType == DefinitionError.LeadingReferenceCycle)
                {
                    found = true;
                }
            }
            Assert.IsTrue(found, "No LeadingReferenceCycle error was found in the returned list.");
        }

        [Test()]
        public void TestMultiDefCycle()
        {
            var a = new NDefinition {
                Name="a",
            };
            var b = new NDefinition {
                Name="b",
            };
            var c = new NDefinition {
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

            var defs = new List<NDefinition> { a, b, c };

            DefinitionChecker dc = new DefinitionChecker();
            var errors = dc.CheckDefinitions(defs).ToList();

            Assert.IsNotEmpty(errors);

            bool found = false;
            foreach (Error ei in errors)
            {
                if (ei.ErrorType == DefinitionError.LeadingReferenceCycle)
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
            var a = new NDefinition {
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
            Assert.IsInstanceOf<DefinitionError>(errors[0]);
            var err = (DefinitionError)errors[0];
            Assert.AreEqual(DefinitionError.NodeHasNoPathFromStart, err.ErrorType);
            Assert.AreSame(n2, err.Node);
        }

        [Test]
        public void TestNoPathToEnd()
        {
            // setup
            var a = new NDefinition {
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
            Assert.IsInstanceOf<DefinitionError>(errors[0]);
            var err = (DefinitionError)errors[0];
            Assert.AreEqual(DefinitionError.NodeHasNoPathToEnd, err.ErrorType);
            Assert.AreSame(n2, err.Node);
        }

        [Test]
        public void TestNextNodeOutsideDefinition1()
        {
            // setup
            var a = new NDefinition() { Name="a" };
            var b = new NDefinition() { Name="b" };
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
            Assert.IsInstanceOf<DefinitionError>(errors[0]);
            var err = (DefinitionError)errors[0];
            Assert.AreEqual(DefinitionError.NextNodeLinksOutsideOfDefinition, err.ErrorType);
            Assert.AreSame(n1, err.Node);
        }

        [Test]
        public void TestNextNodeOutsideDefinition2()
        {
            // setup
            var a = new NDefinition() { Name="a" };
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
            Assert.IsInstanceOf<DefinitionError>(errors[0]);
            var err = (DefinitionError)errors[0];
            Assert.AreEqual(DefinitionError.NextNodeLinksOutsideOfDefinition, err.ErrorType);
            Assert.AreSame(n1, err.Node);
        }

        [Test]
        public void TestStartNodeHasWrongParentDefinition1()
        {
            // setup
            var a = new NDefinition() { Name="a" };
            var b = new NDefinition() { Name="b" };
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
            Assert.IsInstanceOf<DefinitionError>(errors[0]);
            var err = (DefinitionError)errors[0];
            Assert.AreEqual(DefinitionError.StartNodeHasWrongParentDefinition, err.ErrorType);
            Assert.AreSame(n2, err.Node);
        }

        [Test]
        public void TestStartNodeHasWrongParentDefinition2()
        {
            // setup
            var a = new NDefinition() { Name="a" };
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
            Assert.IsInstanceOf<DefinitionError>(errors[0]);
            var err = (DefinitionError)errors[0];
            Assert.AreEqual(DefinitionError.StartNodeHasWrongParentDefinition, err.ErrorType);
            Assert.AreSame(n2, err.Node);
        }

        [Test]
        public void TestEndNodeHasWrongParentDefinition1()
        {
            // setup
            var a = new NDefinition() { Name="a" };
            var b = new NDefinition() { Name="b" };
            var n1 = new CharNode('1');
            var n2 = new CharNode('2');

            a.Nodes.Add(n1);
            a.StartNodes.Add(n1);
            a.EndNodes.Add(n1);
            b.Nodes.Add(n2);
            b.StartNodes.Add(n2);
            b.EndNodes.Add(n2);

            a.EndNodes.Add(n2);

            var dc = new DefinitionChecker();

            // action
            var errorEnu = dc.CheckDefinition(a);
            var errors = new List<Error>(errorEnu);

            // assertions
            Assert.IsNotNull(errorEnu);
            Assert.AreEqual(1, errors.Count);
            Assert.IsInstanceOf<DefinitionError>(errors[0]);
            var err = (DefinitionError)errors[0];
            Assert.AreEqual(DefinitionError.EndNodeHasWrongParentDefinition, err.ErrorType);
            Assert.AreSame(n2, err.Node);
        }

        [Test]
        public void TestEndNodeHasWrongParentDefinition2()
        {
            // setup
            var a = new NDefinition() { Name="a" };
            var n1 = new CharNode('1');
            var n2 = new CharNode('2');

            a.Nodes.Add(n1);
            a.StartNodes.Add(n1);
            a.EndNodes.Add(n1);

            a.EndNodes.Add(n2);

            var dc = new DefinitionChecker();

            // action
            var errorEnu = dc.CheckDefinition(a);
            var errors = new List<Error>(errorEnu);

            // assertions
            Assert.IsNotNull(errorEnu);
            Assert.AreEqual(1, errors.Count);
            Assert.IsInstanceOf<DefinitionError>(errors[0]);
            var err = (DefinitionError)errors[0];
            Assert.AreEqual(DefinitionError.EndNodeHasWrongParentDefinition, err.ErrorType);
            Assert.AreSame(n2, err.Node);
        }

        [Test]
        public void TestCheckDefinitionsNullArgument()
        {
            // setup
            var dc = new DefinitionChecker();

            // action/assertions
            Assert.Throws<ArgumentNullException>(() => dc.CheckDefinitions(null));
        }

        [Test]
        public void TestCheckDefinitionsNullArgument2()
        {
            // setup
            var dc = new DefinitionChecker();

            // action/assertions
            Assert.Throws<ArgumentNullException>(() => dc.CheckDefinitions(null, new List<Error>()));
        }

        [Test]
        public void TestCheckDefinitionsNullArgument3()
        {
            // setup
            var dc = new DefinitionChecker();

            // action/assertions
            Assert.Throws<ArgumentNullException>(
                () => dc.CheckDefinitions(new NDefinition[0], null));
        }

        [Test]
        public void TestCheckDefinitionNullArgument()
        {
            // setup
            var dc = new DefinitionChecker();

            // action/assertions
            Assert.Throws<ArgumentNullException>(() => dc.CheckDefinition(null));
        }

        [Test]
        public void TestCheckDefinitionNullArgument2()
        {
            // setup
            var dc = new DefinitionChecker();

            // action/assertions
            Assert.Throws<ArgumentNullException>(
                () => dc.CheckDefinition(new NDefinition("a"), null));
        }

        [Test]
        public void TestCheckDefinitionNullArgument3()
        {
            // setup
            var dc = new DefinitionChecker();

            // action/assertions
            Assert.Throws<ArgumentNullException>(() => dc.CheckDefinition(null, new List<Error>()));
        }
    }
}

