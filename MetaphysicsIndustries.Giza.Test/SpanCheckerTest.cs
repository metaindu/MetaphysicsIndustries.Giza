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

//        public void TestNodeInWrongDefinition
//        public void TestSpanHasNoSubspans
//        public void TestCycleInSubspans
//        public void TestSpanReused

    }



}

