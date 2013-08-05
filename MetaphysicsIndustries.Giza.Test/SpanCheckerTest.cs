using System;
using NUnit.Framework;
using ScError = MetaphysicsIndustries.Giza.SpanChecker.ScError;

namespace MetaphysicsIndustries.Giza.Test
{
    [TestFixture]
    public class SpanCheckerTest
    {
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
                Value="a"
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

//        public void TestBadEndingNode
//        public void TestBadFollow
//        public void TestNodeInWrongDefinition
//        public void TestSpanHasNoSubspans
//        public void TestCycleInSubspans
//        public void TestSpanReused

    }



}

