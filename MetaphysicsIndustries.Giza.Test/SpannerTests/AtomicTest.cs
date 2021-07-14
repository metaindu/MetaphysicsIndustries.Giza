
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
using NUnit.Framework;
using System.Linq;
using System.Collections.Generic;

namespace MetaphysicsIndustries.Giza.Test
{
    public class AtomicTest
    {
        [Test]
        public void TestAtomic1()
        {
            // setup
            var sequence =
                new Definition(
                    name: "sequence",
                    expr: new Expression(
                        new DefRefSubExpression("letters",
                            isRepeatable: true)));
            var letters =
                new Definition(
                    name: "letters",
                    directives: new[] {DefinitionDirective.Atomic},
                    expr: new Expression(
                        new CharClassSubExpression(
                            CharClass.FromUndelimitedCharClassText("\\l"),
                            isRepeatable: true)));
            var grammar = (new GrammarCompiler()).BuildGrammar(new [] { letters, sequence });
            var sequenceDef = grammar.FindDefinitionByName("sequence");
            var lettersDef = grammar.FindDefinitionByName("letters");
            var spanner = new Spanner(sequenceDef);
            var input = "abc";
            var errors = new List<Error>();

            // action
            var spans = spanner.Process(input.ToCharacterSource(), errors);

            // assertions
            Assert.IsEmpty(errors);
            Assert.IsNotNull(spans);
            Assert.AreEqual(1, spans.Length);
            var s = spans[0];
            Assert.AreSame(sequenceDef, s.DefRef);
            Assert.AreEqual(1, s.Subspans.Count);
            var s2 = s.Subspans[0];
            Assert.AreSame(lettersDef, s2.DefRef);
            Assert.AreEqual(3, s2.Subspans.Count);

            Assert.AreSame(lettersDef.Nodes[0], s2.Subspans[0].Node);
            Assert.AreSame(lettersDef.Nodes[0], s2.Subspans[1].Node);
            Assert.AreSame(lettersDef.Nodes[0], s2.Subspans[2].Node);
            Assert.IsEmpty(s2.Subspans[0].Subspans);
            Assert.IsEmpty(s2.Subspans[1].Subspans);
            Assert.IsEmpty(s2.Subspans[2].Subspans);
            Assert.AreEqual("a", s2.Subspans[0].Value);
            Assert.AreEqual("b", s2.Subspans[1].Value);
            Assert.AreEqual("c", s2.Subspans[2].Value);
        }

        [Test]
        public void TestAtomic2()
        {
            // setup
            var sequence =
                new Definition(
                    name: "sequence",
                    expr: new Expression(
                        new DefRefSubExpression("letters",
                            isRepeatable: true)));
            var letters =
                new Definition(
                    name: "letters",
                    expr: new Expression(
                        new CharClassSubExpression(
                            CharClass.FromUndelimitedCharClassText("\\l"),
                            isRepeatable: true)));
            var grammar = (new GrammarCompiler()).BuildGrammar(new [] { letters, sequence });
            var sequenceDef = grammar.FindDefinitionByName("sequence");
            var lettersDef = grammar.FindDefinitionByName("letters");
            var spanner = new Spanner(sequenceDef);
            var input = "abc";
            var errors = new List<Error>();

            // action
            var spans = spanner.Process(input.ToCharacterSource(), errors);

            // assertions
            Assert.IsEmpty(errors);
            Assert.IsNotNull(spans);
            Assert.AreEqual(4, spans.Length);

            // sequence
            //    |
            // letters('abc')

            //       sequence
            //       |      |
            // letters('a') letters('bc')

            //         sequence
            //         |      |
            // letters('ab') letters('c')

            //                sequence
            //              /     |    \
            //             /      |     \
            // letters('a') letters('b') letters('c')

            Assert.AreEqual(1, spans.Count(x => x.Subspans.Count == 1));    // abc
            Assert.AreEqual(1, spans.Count(x => x.Subspans.Count == 3));    // a b c
            Assert.AreEqual(2, spans.Count(x => x.Subspans.Count == 2));
            Assert.AreEqual(1, spans.Count(
                x =>
                x.Subspans.Count == 2 &&
                x.Subspans[0].Subspans.Count == 1 &&
                x.Subspans[1].Subspans.Count == 2));                    // a bc
            Assert.AreEqual(1, spans.Count(
                x =>
                x.Subspans.Count == 2 &&
                x.Subspans[0].Subspans.Count == 2 &&
                x.Subspans[1].Subspans.Count == 1));                    // ab c

            Span s;
            Span s2;

            s = spans.First(x => x.Subspans.Count == 1);    //abc
            Assert.AreSame(sequenceDef, s.DefRef);
            s2 = s.Subspans[0];
            Assert.AreSame(lettersDef, s2.DefRef);
            Assert.AreEqual(3, s2.Subspans.Count);
            Assert.AreSame(lettersDef.Nodes[0], s2.Subspans[0].Node);
            Assert.AreSame(lettersDef.Nodes[0], s2.Subspans[1].Node);
            Assert.AreSame(lettersDef.Nodes[0], s2.Subspans[2].Node);
            Assert.AreEqual("a", s2.Subspans[0].Value);
            Assert.AreEqual("b", s2.Subspans[1].Value);
            Assert.AreEqual("c", s2.Subspans[2].Value);
            Assert.IsEmpty(s2.Subspans[0].Subspans);
            Assert.IsEmpty(s2.Subspans[1].Subspans);
            Assert.IsEmpty(s2.Subspans[2].Subspans);

            s = spans.First(x => x.Subspans.Count == 2 && x.Subspans[0].Subspans.Count == 1); // a bc
            Assert.AreSame(sequenceDef, s.DefRef);
            s2 = s.Subspans[0];
            Assert.AreSame(lettersDef, s2.DefRef);
            Assert.AreEqual(1, s2.Subspans.Count);
            Assert.AreSame(lettersDef.Nodes[0], s2.Subspans[0].Node);
            Assert.AreEqual("a", s2.Subspans[0].Value);
            Assert.IsEmpty(s2.Subspans[0].Subspans);
            s2 = s.Subspans[1];
            Assert.AreSame(lettersDef, s2.DefRef);
            Assert.AreEqual(2, s2.Subspans.Count);
            Assert.AreSame(lettersDef.Nodes[0], s2.Subspans[0].Node);
            Assert.AreSame(lettersDef.Nodes[0], s2.Subspans[1].Node);
            Assert.AreEqual("b", s2.Subspans[0].Value);
            Assert.AreEqual("c", s2.Subspans[1].Value);
            Assert.IsEmpty(s2.Subspans[0].Subspans);
            Assert.IsEmpty(s2.Subspans[1].Subspans);

            s = spans.First(x => x.Subspans.Count == 2 && x.Subspans[0].Subspans.Count == 2); // ab c
            Assert.AreSame(sequenceDef, s.DefRef);
            s2 = s.Subspans[0];
            Assert.AreSame(lettersDef, s2.DefRef);
            Assert.AreEqual(2, s2.Subspans.Count);
            Assert.AreSame(lettersDef.Nodes[0], s2.Subspans[0].Node);
            Assert.AreSame(lettersDef.Nodes[0], s2.Subspans[1].Node);
            Assert.AreEqual("a", s2.Subspans[0].Value);
            Assert.AreEqual("b", s2.Subspans[1].Value);
            Assert.IsEmpty(s2.Subspans[0].Subspans);
            Assert.IsEmpty(s2.Subspans[1].Subspans);
            s2 = s.Subspans[1];
            Assert.AreSame(lettersDef, s2.DefRef);
            Assert.AreEqual(1, s2.Subspans.Count);
            Assert.AreSame(lettersDef.Nodes[0], s2.Subspans[0].Node);
            Assert.AreEqual("c", s2.Subspans[0].Value);
            Assert.IsEmpty(s2.Subspans[0].Subspans);

            s = spans.First(x => x.Subspans.Count == 3);    // a b c
            Assert.AreSame(sequenceDef, s.DefRef);
            Assert.AreSame(lettersDef, s.Subspans[0].DefRef);
            Assert.AreSame(lettersDef, s.Subspans[1].DefRef);
            Assert.AreSame(lettersDef, s.Subspans[2].DefRef);
            Assert.AreEqual(1, s.Subspans[0].Subspans.Count);
            Assert.AreEqual(1, s.Subspans[1].Subspans.Count);
            Assert.AreEqual(1, s.Subspans[2].Subspans.Count);
            Assert.AreSame(lettersDef.Nodes[0], s.Subspans[0].Subspans[0].Node);
            Assert.AreSame(lettersDef.Nodes[0], s.Subspans[1].Subspans[0].Node);
            Assert.AreSame(lettersDef.Nodes[0], s.Subspans[2].Subspans[0].Node);
            Assert.AreEqual("a", s.Subspans[0].Subspans[0].Value);
            Assert.AreEqual("b", s.Subspans[1].Subspans[0].Value);
            Assert.AreEqual("c", s.Subspans[2].Subspans[0].Value);
            Assert.IsEmpty(s.Subspans[0].Subspans[0].Subspans);
            Assert.IsEmpty(s.Subspans[1].Subspans[0].Subspans);
            Assert.IsEmpty(s.Subspans[2].Subspans[0].Subspans);
        }

    }
}

