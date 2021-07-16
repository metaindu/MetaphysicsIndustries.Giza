
// MetaphysicsIndustries.Giza - A Parsing System
// Copyright (C) 2008-2021 Metaphysics Industries, Inc.
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
using NUnit.Framework;

namespace MetaphysicsIndustries.Giza.Test
{
    [TestFixture]
    public class SpanCheckerTest
    {
        [Test]
        public void TestNoErrors()
        {
            Supergrammar sg = new Supergrammar();
            Span span = new Span() {
                Node=sg.node_subexpr_1_literal,
            };
            span.Subspans.Add(new Span {
                Node=sg.node_literal_0__0027_,
                Value="'"
            });
            span.Subspans.Add(new Span {
                Node=sg.node_literal_1__005E__0027__005C__005C_,
                Value="a"
            });
            span.Subspans.Add(new Span {
                Node=sg.node_literal_1__005E__0027__005C__005C_,
                Value="b"
            });
            span.Subspans.Add(new Span {
                Node=sg.node_literal_1__005E__0027__005C__005C_,
                Value="c"
            });
            span.Subspans.Add(new Span {
                Node=sg.node_literal_5__0027_,
                Value="'"
            });
            SpanChecker sc = new SpanChecker();


            var errors = sc.CheckSpan(span, sg);


            Assert.IsNotNull(errors);
            Assert.AreEqual(0, errors.Count);
        }

        [Test]
        public void TestBadStartingNode()
        {
            Supergrammar sg = new Supergrammar();
            Span span = new Span() {
                Node=sg.node_subexpr_1_literal,
            };
            span.Subspans.Add(new Span {
                Node=sg.node_literal_1__005E__0027__005C__005C_,
                Value="a"
            });
            span.Subspans.Add(new Span {
                Node=sg.node_literal_1__005E__0027__005C__005C_,
                Value="b"
            });
            span.Subspans.Add(new Span {
                Node=sg.node_literal_1__005E__0027__005C__005C_,
                Value="c"
            });
            span.Subspans.Add(new Span {
                Node=sg.node_literal_5__0027_,
                Value="'"
            });
            SpanChecker sc = new SpanChecker();


            var errors = sc.CheckSpan(span, sg);


            Assert.IsNotNull(errors);
            Assert.AreEqual(1, errors.Count);
            Assert.IsInstanceOf<SpanError>(errors[0]);
            var e = (SpanError)errors[0];
            Assert.AreEqual(SpanError.BadStartingNode, e.ErrorType);
            Assert.AreSame(span.Subspans[0], e.Span);
        }

        [Test]
        public void TestBadEndingNode()
        {
            Supergrammar sg = new Supergrammar();
            Span span = new Span() {
                Node=sg.node_subexpr_1_literal,
            };
            span.Subspans.Add(new Span {
                Node=sg.node_literal_0__0027_,
                Value="'"
            });
            span.Subspans.Add(new Span {
                Node=sg.node_literal_1__005E__0027__005C__005C_,
                Value="a"
            });
            span.Subspans.Add(new Span {
                Node=sg.node_literal_1__005E__0027__005C__005C_,
                Value="b"
            });
            span.Subspans.Add(new Span {
                Node=sg.node_literal_1__005E__0027__005C__005C_,
                Value="c"
            });
            SpanChecker sc = new SpanChecker();


            var errors = sc.CheckSpan(span, sg);


            Assert.IsNotNull(errors);
            Assert.AreEqual(1, errors.Count);
            Assert.IsInstanceOf<SpanError>(errors[0]);
            var e = (SpanError)errors[0];
            Assert.AreEqual(SpanError.BadEndingNode, e.ErrorType);
            Assert.AreSame(span.Subspans[3], e.Span);
        }

        [Test]
        public void TestBadFollow()
        {
            Supergrammar sg = new Supergrammar();
            Span span = new Span() {
                Node=sg.node_subexpr_1_literal,
            };
            span.Subspans.Add(new Span {
                Node=sg.node_literal_0__0027_,
                Value="'"
            });
            span.Subspans.Add(new Span {
                Node=sg.node_literal_5__0027_,
                Value="'"
            });
            SpanChecker sc = new SpanChecker();


            var errors = sc.CheckSpan(span, sg);


            Assert.IsNotNull(errors);
            Assert.AreEqual(1, errors.Count);
            Assert.IsInstanceOf<SpanError>(errors[0]);
            var e = (SpanError)errors[0];
            Assert.AreEqual(SpanError.BadFollow, e.ErrorType);
            Assert.AreSame(span.Subspans[1], e.Span);
        }

        [Test]
        public void TestNodeInWrongDefinition1()
        {
            Supergrammar sg = new Supergrammar();
            Span span = new Span() {
                Node=sg.node_subexpr_1_literal,
            };
            span.Subspans.Add(new Span {
                Node=sg.node_literal_0__0027_,
                Value="'"
            });
            span.Subspans.Add(new Span {
                Node=sg.node_literal_1__005E__0027__005C__005C_,
                Value="a"
            });
            span.Subspans.Add(new Span {
                Node=sg.node_charclass_1__005E__005C__005B__005C__005C__005C__005D_,
                Value="b"
            });
            span.Subspans.Add(new Span {
                Node=sg.node_literal_1__005E__0027__005C__005C_,
                Value="c"
            });
            span.Subspans.Add(new Span {
                Node=sg.node_literal_5__0027_,
                Value="'"
            });
            SpanChecker sc = new SpanChecker();


            var errors = sc.CheckSpan(span, sg);


            Assert.IsNotNull(errors);
            Assert.GreaterOrEqual(errors.Count, 1);
            SpanError e = null;
            foreach (Error error in errors)
            {
                if (error.ErrorType == SpanError.NodeInWrongDefinition)
                {
                    Assert.IsInstanceOf<SpanError>(error);
                    e = (error as SpanError);
                    break;
                }
            }
            Assert.IsNotNull(e);
            Assert.AreEqual(SpanError.NodeInWrongDefinition, e.ErrorType);
            Assert.AreSame(span.Subspans[2], e.Span);
        }

        [Test]
        public void TestNodeInWrongDefinition2()
        {
            // we intentionally create a bad set of definitions and nodes to 
            // test the error in isolation. this will prevent bad start, bad 
            // end,  and bad follow errors, yet will still result in a 
            // node-in-wrong-def error. this set of definitions would not pass 
            // DefinitionChecker, nor would it ever be generated by the 
            // definition builders.

            var g = new NGrammar();
            var d0 = new NDefinition("d0");
            var d1 = new NDefinition("d1");
            var d2 = new NDefinition("d2");
            Node n0 = new DefRefNode(d1);
            Node n1 = new CharNode('a');
            Node n2 = new CharNode('b');

            n1.NextNodes.Add(n2);

            g.Definitions.Add(d0);
            g.Definitions.Add(d1);
            g.Definitions.Add(d2);

            d0.Nodes.Add(n0);
            d0.StartNodes.Add(n0);
            d0.EndNodes.Add(n0);

            d1.Nodes.Add(n1);
            d1.StartNodes.Add(n1);
            d1.EndNodes.Add(n2);

            d2.Nodes.Add(n2);
            d2.StartNodes.Add(n2);
            d2.EndNodes.Add(n2);

            Span span = new Span() {
                Node=n0,
            };
            span.Subspans.Add(new Span {
                Node=n1,
                Value="a"
            });
            span.Subspans.Add(new Span {
                Node=n2,
                Value="b"
            });
            SpanChecker sc = new SpanChecker();


            var errors = sc.CheckSpan(span, g);


            Assert.IsNotNull(errors);
            Assert.AreEqual(1, errors.Count);
            Assert.IsInstanceOf<SpanError>(errors[0]);
            var e = (SpanError)errors[0];
            Assert.AreEqual(SpanError.NodeInWrongDefinition, e.ErrorType);
            Assert.AreSame(span.Subspans[1], e.Span);
        }

        [Test]
        public void TestSpanHasNoSubspans()
        {
            Supergrammar sg = new Supergrammar();
            Span span = new Span() {
                Node=sg.node_subexpr_1_literal,
            };
            SpanChecker sc = new SpanChecker();


            var errors = sc.CheckSpan(span, sg);


            Assert.IsNotNull(errors);
            Assert.AreEqual(1, errors.Count);
            Assert.IsInstanceOf<SpanError>(errors[0]);
            var e = (SpanError)errors[0];
            Assert.AreEqual(SpanError.SpanHasNoSubspans, e.ErrorType);
            Assert.AreSame(span, e.Span);
        }

        [Test]
        public void TestCycleInSubspans1()
        {
            Supergrammar sg = new Supergrammar();
            Span span = new Span() {
                Node=sg.node_subexpr_1_literal,
            };
            span.Subspans.Add(new Span {
                Node=sg.node_literal_0__0027_,
                Value="'"
            });
            span.Subspans.Add(new Span {
                Node=sg.node_literal_1__005E__0027__005C__005C_,
                Value="a"
            });
            span.Subspans.Add(new Span {
                Node=sg.node_charclass_1__005E__005C__005B__005C__005C__005C__005D_,
                Value="b"
            });
            span.Subspans.Add(new Span {
                Node=sg.node_literal_1__005E__0027__005C__005C_,
                Value="c"
            });
            span.Subspans.Add(span);
            SpanChecker sc = new SpanChecker();


            var errors = sc.CheckSpan(span, sg);


            Assert.IsNotNull(errors);
            Assert.GreaterOrEqual(errors.Count, 1);
            SpanError e = null;
            foreach (Error error in errors)
            {
                if (error.ErrorType == SpanError.CycleInSubspans)
                {
                    Assert.IsInstanceOf<SpanError>(error);
                    e = (error as SpanError);
                    break;
                }
            }
            Assert.IsNotNull(e);
            Assert.AreEqual(SpanError.CycleInSubspans, e.ErrorType);
            Assert.AreSame(span, e.Span);
            Assert.AreSame(span.Subspans[4], e.Span);
        }

        [Test]
        public void TestCycleInSubspans2()
        {
            // we intentionally create a bad set of definitions and nodes to 
            // test the error in isolation. this will prevent bad start, 
            // bad end, and bad follow errors, yet will still result in a 
            // cycle-in-subspans error. this set of definitions would not pass 
            // DefinitionChecker, nor would it ever be generated by the 
            // definition builders.

            var g = new NGrammar();
            var d0 = new NDefinition("d0");
            Node n0 = new CharNode('a');
            Node n1 = new DefRefNode(d0);

            n0.NextNodes.Add(n1);

            g.Definitions.Add(d0);

            d0.Nodes.Add(n0);
            d0.Nodes.Add(n1);
            d0.StartNodes.Add(n0);
            d0.EndNodes.Add(n1);

            Span span = new Span() {
                Node=n1,
            };
            span.Subspans.Add(new Span {
                Node=n0,
                Value="a"
            });
            span.Subspans.Add(span);
            SpanChecker sc = new SpanChecker();


            var errors = sc.CheckSpan(span, g);


            Assert.IsNotNull(errors);
            Assert.AreEqual(1, errors.Count);
            Assert.IsInstanceOf<SpanError>(errors[0]);
            var e = (SpanError)errors[0];
            Assert.AreEqual(SpanError.CycleInSubspans, e.ErrorType);
            Assert.AreSame(span, e.Span);
            Assert.AreSame(span.Subspans[1], e.Span);
        }
    }
}

