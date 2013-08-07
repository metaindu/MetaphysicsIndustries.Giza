using System;
using NUnit.Framework;
using ScError = MetaphysicsIndustries.Giza.SpanChecker.ScError;

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
                Node=sg.node_literal_1__005E__005C__005C__0027_,
                Value="a"
            });
            span.Subspans.Add(new Span {
                Node=sg.node_literal_1__005E__005C__005C__0027_,
                Value="b"
            });
            span.Subspans.Add(new Span {
                Node=sg.node_literal_1__005E__005C__005C__0027_,
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
                Node=sg.node_literal_1__005E__005C__005C__0027_,
                Value="a"
            });
            span.Subspans.Add(new Span {
                Node=sg.node_literal_1__005E__005C__005C__0027_,
                Value="b"
            });
            span.Subspans.Add(new Span {
                Node=sg.node_literal_1__005E__005C__005C__0027_,
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
            Assert.IsInstanceOf<ScError>(errors[0]);
            var e = (ScError)errors[0];
            Assert.AreEqual(ScError.BadStartingNode, e.ErrorType);
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
                Node=sg.node_literal_1__005E__005C__005C__0027_,
                Value="a"
            });
            span.Subspans.Add(new Span {
                Node=sg.node_literal_1__005E__005C__005C__0027_,
                Value="b"
            });
            span.Subspans.Add(new Span {
                Node=sg.node_literal_1__005E__005C__005C__0027_,
                Value="c"
            });
            SpanChecker sc = new SpanChecker();


            var errors = sc.CheckSpan(span, sg);


            Assert.IsNotNull(errors);
            Assert.AreEqual(1, errors.Count);
            Assert.IsInstanceOf<ScError>(errors[0]);
            var e = (ScError)errors[0];
            Assert.AreEqual(ScError.BadEndingNode, e.ErrorType);
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
            Assert.IsInstanceOf<ScError>(errors[0]);
            var e = (ScError)errors[0];
            Assert.AreEqual(ScError.BadFollow, e.ErrorType);
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
                Node=sg.node_literal_1__005E__005C__005C__0027_,
                Value="a"
            });
            span.Subspans.Add(new Span {
                Node=sg.node_charclass_1__005E__005C__005C__005C__005B__005C__005D_,
                Value="b"
            });
            span.Subspans.Add(new Span {
                Node=sg.node_literal_1__005E__005C__005C__0027_,
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
            ScError e = null;
            foreach (Error error in errors)
            {
                if (error.ErrorType == ScError.NodeInWrongDefinition)
                {
                    Assert.IsInstanceOf<ScError>(error);
                    e = (error as ScError);
                    break;
                }
            }
            Assert.IsNotNull(e);
            Assert.AreEqual(ScError.NodeInWrongDefinition, e.ErrorType);
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

            Grammar g = new Grammar();
            Definition d0 = new Definition("d0");
            Definition d1 = new Definition("d1");
            Definition d2 = new Definition("d2");
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
            Assert.IsInstanceOf<ScError>(errors[0]);
            var e = (ScError)errors[0];
            Assert.AreEqual(ScError.NodeInWrongDefinition, e.ErrorType);
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
            Assert.IsInstanceOf<ScError>(errors[0]);
            var e = (ScError)errors[0];
            Assert.AreEqual(ScError.SpanHasNoSubspans, e.ErrorType);
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
                Node=sg.node_literal_1__005E__005C__005C__0027_,
                Value="a"
            });
            span.Subspans.Add(new Span {
                Node=sg.node_charclass_1__005E__005C__005C__005C__005B__005C__005D_,
                Value="b"
            });
            span.Subspans.Add(new Span {
                Node=sg.node_literal_1__005E__005C__005C__0027_,
                Value="c"
            });
            span.Subspans.Add(span);
            SpanChecker sc = new SpanChecker();


            var errors = sc.CheckSpan(span, sg);


            Assert.IsNotNull(errors);
            Assert.GreaterOrEqual(errors.Count, 1);
            ScError e = null;
            foreach (Error error in errors)
            {
                if (error.ErrorType == ScError.CycleInSubspans)
                {
                    Assert.IsInstanceOf<ScError>(error);
                    e = (error as ScError);
                    break;
                }
            }
            Assert.IsNotNull(e);
            Assert.AreEqual(ScError.CycleInSubspans, e.ErrorType);
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

            Grammar g = new Grammar();
            Definition d0 = new Definition("d0");
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
            Assert.IsInstanceOf<ScError>(errors[0]);
            var e = (ScError)errors[0];
            Assert.AreEqual(ScError.CycleInSubspans, e.ErrorType);
            Assert.AreSame(span, e.Span);
            Assert.AreSame(span.Subspans[1], e.Span);
        }
    }
}

