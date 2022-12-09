
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
using System.Linq;

namespace MetaphysicsIndustries.Giza.Test
{
    [TestFixture]
    public class CharacterSourceTest
    {
        [Test]
        public void TestReadInOrder()
        {
            var s = "One two\r\nthree\n";
            //index  0123456 7 890123 45
            //line   1111111 1 122222 23
            //column 1234567 8 912345 61
            var cs = new CharacterSource(s);

            Assert.That(cs.CurrentPosition.Index, Is.EqualTo(0));
            Assert.That(cs.CurrentPosition.Line, Is.EqualTo(1));
            Assert.That(cs.CurrentPosition.Column, Is.EqualTo(1));
            var ies = cs.Peek();
            Assert.IsFalse(ies.EndOfInput);
            Assert.IsNotNull(ies.Errors);
            Assert.IsEmpty(ies.Errors);
            Assert.IsNotNull(ies.InputElements);
            Assert.That(ies.InputElements.Count(), Is.EqualTo(1));
            Assert.That(ies.InputElements.First().Position,
                Is.EqualTo(cs.CurrentPosition));
            Assert.That(ies.InputElements.First().Value, Is.EqualTo('O'));

            ies = cs.GetNextValue();
            Assert.IsFalse(ies.EndOfInput);
            Assert.IsNotNull(ies.Errors);
            Assert.IsEmpty(ies.Errors);
            Assert.IsNotNull(ies.InputElements);
            Assert.That(ies.InputElements.Count(), Is.EqualTo(1));
            var ch = ies.InputElements.First();

            Assert.That(ch.Position.Index, Is.EqualTo(0));
            Assert.That(ch.Position.Line, Is.EqualTo(1));
            Assert.That(ch.Position.Column, Is.EqualTo(1));
            Assert.That(ch.Value, Is.EqualTo('O'));

            Assert.That(cs.CurrentPosition.Index, Is.EqualTo(1));
            Assert.That(cs.CurrentPosition.Line, Is.EqualTo(1));
            Assert.That(cs.CurrentPosition.Column, Is.EqualTo(2));
            ies = cs.Peek();
            Assert.IsFalse(ies.EndOfInput);
            Assert.IsNotNull(ies.Errors);
            Assert.IsEmpty(ies.Errors);
            Assert.IsNotNull(ies.InputElements);
            Assert.That(ies.InputElements.Count(), Is.EqualTo(1));
            Assert.That(ies.InputElements.First().Position,
                Is.EqualTo(cs.CurrentPosition));
            Assert.That(ies.InputElements.First().Value, Is.EqualTo('n'));

            ies = cs.GetNextValue();
            Assert.IsFalse(ies.EndOfInput);
            Assert.IsNotNull(ies.Errors);
            Assert.IsEmpty(ies.Errors);
            Assert.IsNotNull(ies.InputElements);
            Assert.That(ies.InputElements.Count(), Is.EqualTo(1));
            ch = ies.InputElements.First();

            Assert.That(ch.Position.Index, Is.EqualTo(1));
            Assert.That(ch.Position.Line, Is.EqualTo(1));
            Assert.That(ch.Position.Column, Is.EqualTo(2));
            Assert.That(ch.Value, Is.EqualTo('n'));

            ies = cs.GetNextValue();
            Assert.IsFalse(ies.EndOfInput);
            Assert.IsNotNull(ies.Errors);
            Assert.IsEmpty(ies.Errors);
            Assert.IsNotNull(ies.InputElements);
            Assert.That(ies.InputElements.Count(), Is.EqualTo(1));
            ch = ies.InputElements.First();

            Assert.That(ch.Position.Index, Is.EqualTo(2));
            Assert.That(ch.Position.Line, Is.EqualTo(1));
            Assert.That(ch.Position.Column, Is.EqualTo(3));
            Assert.That(ch.Value, Is.EqualTo('e'));

            ies = cs.GetNextValue();
            Assert.IsFalse(ies.EndOfInput);
            Assert.IsNotNull(ies.Errors);
            Assert.IsEmpty(ies.Errors);
            Assert.IsNotNull(ies.InputElements);
            Assert.That(ies.InputElements.Count(), Is.EqualTo(1));
            ch = ies.InputElements.First();

            Assert.That(ch.Position.Index, Is.EqualTo(3));
            Assert.That(ch.Position.Line, Is.EqualTo(1));
            Assert.That(ch.Position.Column, Is.EqualTo(4));
            Assert.That(ch.Value, Is.EqualTo(' '));

            ies = cs.GetNextValue();
            Assert.IsFalse(ies.EndOfInput);
            Assert.IsNotNull(ies.Errors);
            Assert.IsEmpty(ies.Errors);
            Assert.IsNotNull(ies.InputElements);
            Assert.That(ies.InputElements.Count(), Is.EqualTo(1));
            ch = ies.InputElements.First();

            Assert.That(ch.Position.Index, Is.EqualTo(4));
            Assert.That(ch.Position.Line, Is.EqualTo(1));
            Assert.That(ch.Position.Column, Is.EqualTo(5));
            Assert.That(ch.Value, Is.EqualTo('t'));

            ies = cs.GetNextValue();
            Assert.IsFalse(ies.EndOfInput);
            Assert.IsNotNull(ies.Errors);
            Assert.IsEmpty(ies.Errors);
            Assert.IsNotNull(ies.InputElements);
            Assert.That(ies.InputElements.Count(), Is.EqualTo(1));
            ch = ies.InputElements.First();

            Assert.That(ch.Position.Index, Is.EqualTo(5));
            Assert.That(ch.Position.Line, Is.EqualTo(1));
            Assert.That(ch.Position.Column, Is.EqualTo(6));
            Assert.That(ch.Value, Is.EqualTo('w'));

            ies = cs.GetNextValue();
            Assert.IsFalse(ies.EndOfInput);
            Assert.IsNotNull(ies.Errors);
            Assert.IsEmpty(ies.Errors);
            Assert.IsNotNull(ies.InputElements);
            Assert.That(ies.InputElements.Count(), Is.EqualTo(1));
            ch = ies.InputElements.First();

            Assert.That(ch.Position.Index, Is.EqualTo(6));
            Assert.That(ch.Position.Line, Is.EqualTo(1));
            Assert.That(ch.Position.Column, Is.EqualTo(7));
            Assert.That(ch.Value, Is.EqualTo('o'));

            ies = cs.GetNextValue();
            Assert.IsFalse(ies.EndOfInput);
            Assert.IsNotNull(ies.Errors);
            Assert.IsEmpty(ies.Errors);
            Assert.IsNotNull(ies.InputElements);
            Assert.That(ies.InputElements.Count(), Is.EqualTo(1));
            ch = ies.InputElements.First();

            Assert.That(ch.Position.Index, Is.EqualTo(7));
            Assert.That(ch.Position.Line, Is.EqualTo(1));
            Assert.That(ch.Position.Column, Is.EqualTo(8));
            Assert.That(ch.Value, Is.EqualTo('\r'));

            Assert.That(cs.CurrentPosition.Index, Is.EqualTo(8));
            Assert.That(cs.CurrentPosition.Line, Is.EqualTo(1));
            Assert.That(cs.CurrentPosition.Column, Is.EqualTo(9));
            ies = cs.Peek();
            Assert.IsFalse(ies.EndOfInput);
            Assert.IsNotNull(ies.Errors);
            Assert.IsEmpty(ies.Errors);
            Assert.IsNotNull(ies.InputElements);
            Assert.That(ies.InputElements.Count(), Is.EqualTo(1));
            Assert.That(ies.InputElements.First().Position,
                Is.EqualTo(cs.CurrentPosition));
            Assert.That(ies.InputElements.First().Value, Is.EqualTo('\n'));

            ies = cs.GetNextValue();
            Assert.IsFalse(ies.EndOfInput);
            Assert.IsNotNull(ies.Errors);
            Assert.IsEmpty(ies.Errors);
            Assert.IsNotNull(ies.InputElements);
            Assert.That(ies.InputElements.Count(), Is.EqualTo(1));
            ch = ies.InputElements.First();

            Assert.That(ch.Position.Index, Is.EqualTo(8));
            Assert.That(ch.Position.Line, Is.EqualTo(1));
            Assert.That(ch.Position.Column, Is.EqualTo(9));
            Assert.That(ch.Value, Is.EqualTo('\n'));

            Assert.That(cs.CurrentPosition.Index, Is.EqualTo(9));
            Assert.That(cs.CurrentPosition.Line, Is.EqualTo(2));
            Assert.That(cs.CurrentPosition.Column, Is.EqualTo(1));
            ies = cs.Peek();
            Assert.IsFalse(ies.EndOfInput);
            Assert.IsNotNull(ies.Errors);
            Assert.IsEmpty(ies.Errors);
            Assert.IsNotNull(ies.InputElements);
            Assert.That(ies.InputElements.Count(), Is.EqualTo(1));
            Assert.That(ies.InputElements.First().Position,
                Is.EqualTo(cs.CurrentPosition));
            Assert.That(ies.InputElements.First().Value, Is.EqualTo('t'));

            ies = cs.GetNextValue();
            Assert.IsFalse(ies.EndOfInput);
            Assert.IsNotNull(ies.Errors);
            Assert.IsEmpty(ies.Errors);
            Assert.IsNotNull(ies.InputElements);
            Assert.That(ies.InputElements.Count(), Is.EqualTo(1));
            ch = ies.InputElements.First();

            Assert.That(ch.Position.Index, Is.EqualTo(9));
            Assert.That(ch.Position.Line, Is.EqualTo(2));
            Assert.That(ch.Position.Column, Is.EqualTo(1));
            Assert.That(ch.Value, Is.EqualTo('t'));

            ies = cs.GetNextValue();
            Assert.IsFalse(ies.EndOfInput);
            Assert.IsNotNull(ies.Errors);
            Assert.IsEmpty(ies.Errors);
            Assert.IsNotNull(ies.InputElements);
            Assert.That(ies.InputElements.Count(), Is.EqualTo(1));
            ch = ies.InputElements.First();

            Assert.That(ch.Position.Index, Is.EqualTo(10));
            Assert.That(ch.Position.Line, Is.EqualTo(2));
            Assert.That(ch.Position.Column, Is.EqualTo(2));
            Assert.That(ch.Value, Is.EqualTo('h'));

            ies = cs.GetNextValue();
            Assert.IsFalse(ies.EndOfInput);
            Assert.IsNotNull(ies.Errors);
            Assert.IsEmpty(ies.Errors);
            Assert.IsNotNull(ies.InputElements);
            Assert.That(ies.InputElements.Count(), Is.EqualTo(1));
            ch = ies.InputElements.First();

            Assert.That(ch.Position.Index, Is.EqualTo(11));
            Assert.That(ch.Position.Line, Is.EqualTo(2));
            Assert.That(ch.Position.Column, Is.EqualTo(3));
            Assert.That(ch.Value, Is.EqualTo('r'));

            ies = cs.GetNextValue();
            Assert.IsFalse(ies.EndOfInput);
            Assert.IsNotNull(ies.Errors);
            Assert.IsEmpty(ies.Errors);
            Assert.IsNotNull(ies.InputElements);
            Assert.That(ies.InputElements.Count(), Is.EqualTo(1));
            ch = ies.InputElements.First();

            Assert.That(ch.Position.Index, Is.EqualTo(12));
            Assert.That(ch.Position.Line, Is.EqualTo(2));
            Assert.That(ch.Position.Column, Is.EqualTo(4));
            Assert.That(ch.Value, Is.EqualTo('e'));

            Assert.That(cs.CurrentPosition.Index, Is.EqualTo(13));
            Assert.That(cs.CurrentPosition.Line, Is.EqualTo(2));
            Assert.That(cs.CurrentPosition.Column, Is.EqualTo(5));
            ies = cs.Peek();
            Assert.IsFalse(ies.EndOfInput);
            Assert.IsNotNull(ies.Errors);
            Assert.IsEmpty(ies.Errors);
            Assert.IsNotNull(ies.InputElements);
            Assert.That(ies.InputElements.Count(), Is.EqualTo(1));
            Assert.That(ies.InputElements.First().Position,
                Is.EqualTo(cs.CurrentPosition));
            Assert.That(ies.InputElements.First().Value, Is.EqualTo('e'));

            ies = cs.GetNextValue();
            Assert.IsFalse(ies.EndOfInput);
            Assert.IsNotNull(ies.Errors);
            Assert.IsEmpty(ies.Errors);
            Assert.IsNotNull(ies.InputElements);
            Assert.That(ies.InputElements.Count(), Is.EqualTo(1));
            ch = ies.InputElements.First();

            Assert.That(ch.Position.Index, Is.EqualTo(13));
            Assert.That(ch.Position.Line, Is.EqualTo(2));
            Assert.That(ch.Position.Column, Is.EqualTo(5));
            Assert.That(ch.Value, Is.EqualTo('e'));

            Assert.That(cs.CurrentPosition.Index, Is.EqualTo(14));
            Assert.That(cs.CurrentPosition.Line, Is.EqualTo(2));
            Assert.That(cs.CurrentPosition.Column, Is.EqualTo(6));
            ies = cs.Peek();
            Assert.IsFalse(ies.EndOfInput);
            Assert.IsNotNull(ies.Errors);
            Assert.IsEmpty(ies.Errors);
            Assert.IsNotNull(ies.InputElements);
            Assert.That(ies.InputElements.Count(), Is.EqualTo(1));
            Assert.That(ies.InputElements.First().Position,
                Is.EqualTo(cs.CurrentPosition));
            Assert.That(ies.InputElements.First().Value, Is.EqualTo('\n'));

            Assert.IsFalse(cs.IsAtEnd);

            ies = cs.GetNextValue();
            Assert.IsFalse(ies.EndOfInput);
            Assert.IsNotNull(ies.Errors);
            Assert.IsEmpty(ies.Errors);
            Assert.IsNotNull(ies.InputElements);
            Assert.That(ies.InputElements.Count(), Is.EqualTo(1));
            ch = ies.InputElements.First();

            Assert.That(ch.Position.Index, Is.EqualTo(14));
            Assert.That(ch.Position.Line, Is.EqualTo(2));
            Assert.That(ch.Position.Column, Is.EqualTo(6));
            Assert.That(ch.Value, Is.EqualTo('\n'));

            Assert.That(cs.CurrentPosition.Index, Is.EqualTo(15));
            Assert.That(cs.CurrentPosition.Line, Is.EqualTo(3));
            Assert.That(cs.CurrentPosition.Column, Is.EqualTo(1));

            Assert.IsTrue(cs.IsAtEnd);
        }
    }
}

