using System;
using NUnit.Framework;
using System.Collections.Generic;

namespace MetaphysicsIndustries.Giza.Test.SpannerTests
{
    [TestFixture]
    public class SequenceTest
    {
        Spanner spanner;
        Definition sequenceDef;
        List<Error> errors;

        [SetUp]
        public void Setup()
        {
            sequenceDef =
                new Definition(
                    name: "sequence",
                    nodes: new [] {
                        new CharNode('a', "abc"),
                        new CharNode('b', "abc"),
                        new CharNode('c', "abc"),
                    },
                    nexts: new [] { 0, 1, 1, 2 },
                    startNodes: new [] { 0 },
                    endNodes: new [] { 2 }
                );
            var grammar = new Grammar(sequenceDef);
            spanner = new Spanner(sequenceDef);
            errors = new List<Error>();
        }

        [Test]
        public void TestSequence()
        {
            // action
            var spans = spanner.Process("abc".ToCharacterSource(), errors);

            // assertions
            Assert.IsEmpty(errors);
            Assert.IsNotNull(spans);
            Assert.AreEqual(1, spans.Length);
            Assert.AreSame(sequenceDef, spans[0].DefRef);
            Assert.AreEqual(3, spans[0].Subspans.Count);
            Assert.AreSame(sequenceDef.Nodes[0], spans[0].Subspans[0].Node);
            Assert.AreSame(sequenceDef.Nodes[1], spans[0].Subspans[1].Node);
            Assert.AreSame(sequenceDef.Nodes[2], spans[0].Subspans[2].Node);
            Assert.AreEqual("a", spans[0].Subspans[0].Value);
            Assert.AreEqual("b", spans[0].Subspans[1].Value);
            Assert.AreEqual("c", spans[0].Subspans[2].Value);
            Assert.IsEmpty(spans[0].Subspans[0].Subspans);
            Assert.IsEmpty(spans[0].Subspans[1].Subspans);
            Assert.IsEmpty(spans[0].Subspans[2].Subspans);
        }

        [Test]
        public void TestTooMuchInput()
        {
            // action
            var spans = spanner.Process("abcd".ToCharacterSource(), errors);

            // assertions
            Assert.IsEmpty(spans);
            Assert.AreEqual(1, errors.Count);
            Assert.IsInstanceOf<SpannerError>(errors[0]);
            var se = (SpannerError)errors[0];
            Assert.AreEqual(SpannerError.ExcessRemainingInput, se.ErrorType);
            Assert.AreEqual('d', se.OffendingInputElement.Value);
            Assert.AreEqual(new InputPosition(3, 1, 4), se.Position);
        }

        [Test]
        public void TestMismatch1()
        {
            // action
            var spans = spanner.Process("acb".ToCharacterSource(), errors);

            // assertions
            Assert.IsEmpty(spans);
            Assert.AreEqual(1, errors.Count);
            Assert.IsInstanceOf<SpannerError>(errors[0]);
            var se = (SpannerError)errors[0];
            Assert.AreEqual(SpannerError.InvalidInputElement, se.ErrorType);
            Assert.AreEqual('c', se.OffendingInputElement.Value);
            Assert.AreEqual(new InputPosition(1, 1, 2), se.Position);
        }

        [Test]
        public void TestMismatch2()
        {
            // action
            var spans = spanner.Process("abd".ToCharacterSource(), errors);

            // assertions
            Assert.IsEmpty(spans);
            Assert.AreEqual(1, errors.Count);
            Assert.IsInstanceOf<SpannerError>(errors[0]);
            var se = (SpannerError)errors[0];
            Assert.AreEqual(SpannerError.InvalidInputElement, se.ErrorType);
            Assert.AreEqual('d', se.OffendingInputElement.Value);
            Assert.AreEqual(new InputPosition(2, 1, 3), se.Position);
        }

        [Test]
        public void TestMismatch3()
        {
            // action
            var spans = spanner.Process("abbc".ToCharacterSource(), errors);

            // assertions
            Assert.IsEmpty(spans);
            Assert.AreEqual(1, errors.Count);
            Assert.IsInstanceOf<SpannerError>(errors[0]);
            var se = (SpannerError)errors[0];
            Assert.AreEqual(SpannerError.InvalidInputElement, se.ErrorType);
            Assert.AreEqual('b', se.OffendingInputElement.Value);
            Assert.AreEqual(new InputPosition(2, 1, 3), se.Position);
        }

        [Test]
        public void TestMismatch4()
        {
            // action
            var spans = spanner.Process("bac".ToCharacterSource(), errors);

            // assertions
            Assert.IsEmpty(spans);
            Assert.AreEqual(1, errors.Count);
            Assert.IsInstanceOf<SpannerError>(errors[0]);
            var se = (SpannerError)errors[0];
            Assert.AreEqual(SpannerError.InvalidInputElement, se.ErrorType);
            Assert.AreEqual('b', se.OffendingInputElement.Value);
            Assert.AreEqual(new InputPosition(0, 1, 1), se.Position);
        }

        [Test]
        public void TestEndOfInput()
        {
            // action
            var spans = spanner.Process("ab".ToCharacterSource(), errors);

            // assertions
            Assert.IsEmpty(spans);
            Assert.AreEqual(1, errors.Count);
            Assert.IsInstanceOf<SpannerError>(errors[0]);
            var se = (SpannerError)errors[0];
            Assert.AreEqual(SpannerError.UnexpectedEndOfInput, se.ErrorType);
            Assert.AreEqual(new InputPosition(2, 1, 3), se.Position);
        }
    }
}

