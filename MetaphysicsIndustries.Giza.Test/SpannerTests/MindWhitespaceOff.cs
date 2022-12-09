
// MetaphysicsIndustries.Giza - A Parsing System
// Copyright (C) 2008-2022 Metaphysics Industries, Inc.
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
using System.Collections.Generic;

namespace MetaphysicsIndustries.Giza.Test.SpannerTests
{
    [TestFixture]
    public class MindWhitespaceOff
    {
        NDefinition def;
        Spanner spanner;
        List<Error> errors;

        [SetUp]
        public void Setup()
        {
            def =
                new NDefinition(
                    name: "def",
                    nodes: new [] {
                        new CharNode('a', "abc"),
                        new CharNode('b', "abc"),
                        new CharNode('c', "abc"),
                    },
                    nexts: new [] { 0, 1, 1, 2 },
                    startNodes: new [] { 0 },
                    endNodes: new [] { 2 },
                    directives: new DefinitionDirective[] {
                    }
                );
            var grammar = new NGrammar(def);
            spanner = new Spanner(def);
            errors = new List<Error>();
        }

        [Test]
        public void TestSuccess()
        {
            // action
            var spans = spanner.Process("abc".ToCharacterSource(), errors);

            // assertions
            Assert.IsEmpty(errors);
            Assert.That(spans.Length, Is.EqualTo(1));
            Assert.That(spans[0].CollectValue(), Is.EqualTo("abc"));
        }

        [Test]
        public void TestLeadingWhitespace()
        {
            // action
            var spans = spanner.Process(" abc".ToCharacterSource(), errors);

            // assertions
            Assert.IsEmpty(errors);
            Assert.That(spans.Length, Is.EqualTo(1));
            Assert.That(spans[0].CollectValue(), Is.EqualTo("abc"));
        }

        [Test]
        public void TestTrailingWhitespace()
        {
            // action
            var spans = spanner.Process("abc ".ToCharacterSource(), errors);

            // assertions
            Assert.IsEmpty(errors);
            Assert.That(spans.Length, Is.EqualTo(1));
            Assert.That(spans[0].CollectValue(), Is.EqualTo("abc"));
        }

        [Test]
        public void TestInnerWhitespace()
        {
            // action
            var spans = spanner.Process("a b c".ToCharacterSource(), errors);

            // assertions
            Assert.IsEmpty(errors);
            Assert.That(spans.Length, Is.EqualTo(1));
            Assert.That(spans[0].CollectValue(), Is.EqualTo("abc"));
        }

        [Test]
        public void TestTab()
        {
            // action
            var spans = spanner.Process("\tabc".ToCharacterSource(), errors);

            // assertions
            Assert.IsEmpty(errors);
            Assert.That(spans.Length, Is.EqualTo(1));
            Assert.That(spans[0].CollectValue(), Is.EqualTo("abc"));
        }

        [Test]
        public void TestNewline()
        {
            // action
            var spans = spanner.Process("\nabc".ToCharacterSource(), errors);

            // assertions
            Assert.IsEmpty(errors);
            Assert.That(spans.Length, Is.EqualTo(1));
            Assert.That(spans[0].CollectValue(), Is.EqualTo("abc"));
        }

        [Test]
        public void TestCarriageReturn()
        {
            // action
            var spans = spanner.Process("\rabc".ToCharacterSource(), errors);

            // assertions
            Assert.IsEmpty(errors);
            Assert.That(spans.Length, Is.EqualTo(1));
            Assert.That(spans[0].CollectValue(), Is.EqualTo("abc"));
        }
    }
}

