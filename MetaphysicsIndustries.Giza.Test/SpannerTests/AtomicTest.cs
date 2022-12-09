
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
            var grammar = (new GrammarCompiler()).Compile(new [] { letters, sequence });
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
            Assert.That(spans.Length, Is.EqualTo(1));
            var s = spans[0];
            Assert.That(s.DefRef, Is.SameAs(sequenceDef));
            Assert.That(s.Subspans.Count, Is.EqualTo(1));
            var s2 = s.Subspans[0];
            Assert.That(s2.DefRef, Is.SameAs(lettersDef));
            Assert.That(s2.Subspans.Count, Is.EqualTo(3));

            Assert.That(s2.Subspans[0].Node, Is.SameAs(lettersDef.Nodes[0]));
            Assert.That(s2.Subspans[1].Node, Is.SameAs(lettersDef.Nodes[0]));
            Assert.That(s2.Subspans[2].Node, Is.SameAs(lettersDef.Nodes[0]));
            Assert.IsEmpty(s2.Subspans[0].Subspans);
            Assert.IsEmpty(s2.Subspans[1].Subspans);
            Assert.IsEmpty(s2.Subspans[2].Subspans);
            Assert.That(s2.Subspans[0].Value, Is.EqualTo("a"));
            Assert.That(s2.Subspans[1].Value, Is.EqualTo("b"));
            Assert.That(s2.Subspans[2].Value, Is.EqualTo("c"));
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
            var grammar = (new GrammarCompiler()).Compile(new [] { letters, sequence });
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
            Assert.That(spans.Length, Is.EqualTo(4));

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

            Assert.That(spans.Count(x => x.Subspans.Count == 1),
                Is.EqualTo(1));                                      // abc
            Assert.That(spans.Count(x => x.Subspans.Count == 3),
                Is.EqualTo(1));                                      // a b c
            Assert.That(spans.Count(x => x.Subspans.Count == 2),
                Is.EqualTo(2));
            Assert.That(spans.Count(
                x =>
                x.Subspans.Count == 2 &&
                x.Subspans[0].Subspans.Count == 1 &&
                x.Subspans[1].Subspans.Count == 2), Is.EqualTo(1));  // a bc
            Assert.That(spans.Count(
                x =>
                x.Subspans.Count == 2 &&
                x.Subspans[0].Subspans.Count == 2 &&
                x.Subspans[1].Subspans.Count == 1), Is.EqualTo(1));  // ab c

            Span s;
            Span s2;

            s = spans.First(x => x.Subspans.Count == 1);    //abc
            Assert.That(s.DefRef, Is.SameAs(sequenceDef));
            s2 = s.Subspans[0];
            Assert.That(s2.DefRef, Is.SameAs(lettersDef));
            Assert.That(s2.Subspans.Count, Is.EqualTo(3));
            Assert.That(s2.Subspans[0].Node, Is.SameAs(lettersDef.Nodes[0]));
            Assert.That(s2.Subspans[1].Node, Is.SameAs(lettersDef.Nodes[0]));
            Assert.That(s2.Subspans[2].Node, Is.SameAs(lettersDef.Nodes[0]));
            Assert.That(s2.Subspans[0].Value, Is.EqualTo("a"));
            Assert.That(s2.Subspans[1].Value, Is.EqualTo("b"));
            Assert.That(s2.Subspans[2].Value, Is.EqualTo("c"));
            Assert.IsEmpty(s2.Subspans[0].Subspans);
            Assert.IsEmpty(s2.Subspans[1].Subspans);
            Assert.IsEmpty(s2.Subspans[2].Subspans);

            s = spans.First(x => x.Subspans.Count == 2 && x.Subspans[0].Subspans.Count == 1); // a bc
            Assert.That(s.DefRef, Is.SameAs(sequenceDef));
            s2 = s.Subspans[0];
            Assert.That(s2.DefRef, Is.SameAs(lettersDef));
            Assert.That(s2.Subspans.Count, Is.EqualTo(1));
            Assert.That(s2.Subspans[0].Node, Is.SameAs(lettersDef.Nodes[0]));
            Assert.That(s2.Subspans[0].Value, Is.EqualTo("a"));
            Assert.IsEmpty(s2.Subspans[0].Subspans);
            s2 = s.Subspans[1];
            Assert.That(s2.DefRef, Is.SameAs(lettersDef));
            Assert.That(s2.Subspans.Count, Is.EqualTo(2));
            Assert.That(s2.Subspans[0].Node, Is.SameAs(lettersDef.Nodes[0]));
            Assert.That(s2.Subspans[1].Node, Is.SameAs(lettersDef.Nodes[0]));
            Assert.That(s2.Subspans[0].Value, Is.EqualTo("b"));
            Assert.That(s2.Subspans[1].Value, Is.EqualTo("c"));
            Assert.IsEmpty(s2.Subspans[0].Subspans);
            Assert.IsEmpty(s2.Subspans[1].Subspans);

            s = spans.First(x => x.Subspans.Count == 2 && x.Subspans[0].Subspans.Count == 2); // ab c
            Assert.That(s.DefRef, Is.SameAs(sequenceDef));
            s2 = s.Subspans[0];
            Assert.That(s2.DefRef, Is.SameAs(lettersDef));
            Assert.That(s2.Subspans.Count, Is.EqualTo(2));
            Assert.That(s2.Subspans[0].Node, Is.SameAs(lettersDef.Nodes[0]));
            Assert.That(s2.Subspans[1].Node, Is.SameAs(lettersDef.Nodes[0]));
            Assert.That(s2.Subspans[0].Value, Is.EqualTo("a"));
            Assert.That(s2.Subspans[1].Value, Is.EqualTo("b"));
            Assert.IsEmpty(s2.Subspans[0].Subspans);
            Assert.IsEmpty(s2.Subspans[1].Subspans);
            s2 = s.Subspans[1];
            Assert.That(s2.DefRef, Is.SameAs(lettersDef));
            Assert.That(s2.Subspans.Count, Is.EqualTo(1));
            Assert.That(s2.Subspans[0].Node, Is.SameAs(lettersDef.Nodes[0]));
            Assert.That(s2.Subspans[0].Value, Is.EqualTo("c"));
            Assert.IsEmpty(s2.Subspans[0].Subspans);

            s = spans.First(x => x.Subspans.Count == 3);    // a b c
            Assert.That(s.DefRef, Is.SameAs(sequenceDef));
            Assert.That(s.Subspans[0].DefRef, Is.SameAs(lettersDef));
            Assert.That(s.Subspans[1].DefRef, Is.SameAs(lettersDef));
            Assert.That(s.Subspans[2].DefRef, Is.SameAs(lettersDef));
            Assert.That(s.Subspans[0].Subspans.Count, Is.EqualTo(1));
            Assert.That(s.Subspans[1].Subspans.Count, Is.EqualTo(1));
            Assert.That(s.Subspans[2].Subspans.Count, Is.EqualTo(1));
            Assert.That(s.Subspans[0].Subspans[0].Node,
             Is.SameAs(lettersDef.Nodes[0]));
            Assert.That(s.Subspans[1].Subspans[0].Node,
                Is.SameAs(lettersDef.Nodes[0]));
            Assert.That(s.Subspans[2].Subspans[0].Node,
                Is.SameAs(lettersDef.Nodes[0]));
            Assert.That(s.Subspans[0].Subspans[0].Value, Is.EqualTo("a"));
            Assert.That(s.Subspans[1].Subspans[0].Value, Is.EqualTo("b"));
            Assert.That(s.Subspans[2].Subspans[0].Value, Is.EqualTo("c"));
            Assert.IsEmpty(s.Subspans[0].Subspans[0].Subspans);
            Assert.IsEmpty(s.Subspans[1].Subspans[0].Subspans);
            Assert.IsEmpty(s.Subspans[2].Subspans[0].Subspans);
        }

    }
}

