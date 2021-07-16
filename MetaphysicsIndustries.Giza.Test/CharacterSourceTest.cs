
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

            Assert.AreEqual(0, cs.CurrentPosition.Index);
            Assert.AreEqual(1, cs.CurrentPosition.Line);
            Assert.AreEqual(1, cs.CurrentPosition.Column);
            var ies = cs.Peek();
            Assert.IsFalse(ies.EndOfInput);
            Assert.IsNotNull(ies.Errors);
            Assert.IsEmpty(ies.Errors);
            Assert.IsNotNull(ies.InputElements);
            Assert.AreEqual(1, ies.InputElements.Count());
            Assert.AreEqual(cs.CurrentPosition, ies.InputElements.First().Position);
            Assert.AreEqual('O', ies.InputElements.First().Value);

            ies = cs.GetNextValue();
            Assert.IsFalse(ies.EndOfInput);
            Assert.IsNotNull(ies.Errors);
            Assert.IsEmpty(ies.Errors);
            Assert.IsNotNull(ies.InputElements);
            Assert.AreEqual(1, ies.InputElements.Count());
            var ch = ies.InputElements.First();

            Assert.AreEqual(0, ch.Position.Index);
            Assert.AreEqual(1, ch.Position.Line);
            Assert.AreEqual(1, ch.Position.Column);
            Assert.AreEqual('O', ch.Value);

            Assert.AreEqual(1, cs.CurrentPosition.Index);
            Assert.AreEqual(1, cs.CurrentPosition.Line);
            Assert.AreEqual(2, cs.CurrentPosition.Column);
            ies = cs.Peek();
            Assert.IsFalse(ies.EndOfInput);
            Assert.IsNotNull(ies.Errors);
            Assert.IsEmpty(ies.Errors);
            Assert.IsNotNull(ies.InputElements);
            Assert.AreEqual(1, ies.InputElements.Count());
            Assert.AreEqual(cs.CurrentPosition, ies.InputElements.First().Position);
            Assert.AreEqual('n', ies.InputElements.First().Value);

            ies = cs.GetNextValue();
            Assert.IsFalse(ies.EndOfInput);
            Assert.IsNotNull(ies.Errors);
            Assert.IsEmpty(ies.Errors);
            Assert.IsNotNull(ies.InputElements);
            Assert.AreEqual(1, ies.InputElements.Count());
            ch = ies.InputElements.First();

            Assert.AreEqual(1, ch.Position.Index);
            Assert.AreEqual(1, ch.Position.Line);
            Assert.AreEqual(2, ch.Position.Column);
            Assert.AreEqual('n', ch.Value);

            ies = cs.GetNextValue();
            Assert.IsFalse(ies.EndOfInput);
            Assert.IsNotNull(ies.Errors);
            Assert.IsEmpty(ies.Errors);
            Assert.IsNotNull(ies.InputElements);
            Assert.AreEqual(1, ies.InputElements.Count());
            ch = ies.InputElements.First();

            Assert.AreEqual(2, ch.Position.Index);
            Assert.AreEqual(1, ch.Position.Line);
            Assert.AreEqual(3, ch.Position.Column);
            Assert.AreEqual('e', ch.Value);

            ies = cs.GetNextValue();
            Assert.IsFalse(ies.EndOfInput);
            Assert.IsNotNull(ies.Errors);
            Assert.IsEmpty(ies.Errors);
            Assert.IsNotNull(ies.InputElements);
            Assert.AreEqual(1, ies.InputElements.Count());
            ch = ies.InputElements.First();

            Assert.AreEqual(3, ch.Position.Index);
            Assert.AreEqual(1, ch.Position.Line);
            Assert.AreEqual(4, ch.Position.Column);
            Assert.AreEqual(' ', ch.Value);

            ies = cs.GetNextValue();
            Assert.IsFalse(ies.EndOfInput);
            Assert.IsNotNull(ies.Errors);
            Assert.IsEmpty(ies.Errors);
            Assert.IsNotNull(ies.InputElements);
            Assert.AreEqual(1, ies.InputElements.Count());
            ch = ies.InputElements.First();

            Assert.AreEqual(4, ch.Position.Index);
            Assert.AreEqual(1, ch.Position.Line);
            Assert.AreEqual(5, ch.Position.Column);
            Assert.AreEqual('t', ch.Value);

            ies = cs.GetNextValue();
            Assert.IsFalse(ies.EndOfInput);
            Assert.IsNotNull(ies.Errors);
            Assert.IsEmpty(ies.Errors);
            Assert.IsNotNull(ies.InputElements);
            Assert.AreEqual(1, ies.InputElements.Count());
            ch = ies.InputElements.First();

            Assert.AreEqual(5, ch.Position.Index);
            Assert.AreEqual(1, ch.Position.Line);
            Assert.AreEqual(6, ch.Position.Column);
            Assert.AreEqual('w', ch.Value);

            ies = cs.GetNextValue();
            Assert.IsFalse(ies.EndOfInput);
            Assert.IsNotNull(ies.Errors);
            Assert.IsEmpty(ies.Errors);
            Assert.IsNotNull(ies.InputElements);
            Assert.AreEqual(1, ies.InputElements.Count());
            ch = ies.InputElements.First();

            Assert.AreEqual(6, ch.Position.Index);
            Assert.AreEqual(1, ch.Position.Line);
            Assert.AreEqual(7, ch.Position.Column);
            Assert.AreEqual('o', ch.Value);

            ies = cs.GetNextValue();
            Assert.IsFalse(ies.EndOfInput);
            Assert.IsNotNull(ies.Errors);
            Assert.IsEmpty(ies.Errors);
            Assert.IsNotNull(ies.InputElements);
            Assert.AreEqual(1, ies.InputElements.Count());
            ch = ies.InputElements.First();

            Assert.AreEqual(7, ch.Position.Index);
            Assert.AreEqual(1, ch.Position.Line);
            Assert.AreEqual(8, ch.Position.Column);
            Assert.AreEqual('\r', ch.Value);

            Assert.AreEqual(8, cs.CurrentPosition.Index);
            Assert.AreEqual(1, cs.CurrentPosition.Line);
            Assert.AreEqual(9, cs.CurrentPosition.Column);
            ies = cs.Peek();
            Assert.IsFalse(ies.EndOfInput);
            Assert.IsNotNull(ies.Errors);
            Assert.IsEmpty(ies.Errors);
            Assert.IsNotNull(ies.InputElements);
            Assert.AreEqual(1, ies.InputElements.Count());
            Assert.AreEqual(cs.CurrentPosition, ies.InputElements.First().Position);
            Assert.AreEqual('\n', ies.InputElements.First().Value);

            ies = cs.GetNextValue();
            Assert.IsFalse(ies.EndOfInput);
            Assert.IsNotNull(ies.Errors);
            Assert.IsEmpty(ies.Errors);
            Assert.IsNotNull(ies.InputElements);
            Assert.AreEqual(1, ies.InputElements.Count());
            ch = ies.InputElements.First();

            Assert.AreEqual(8, ch.Position.Index);
            Assert.AreEqual(1, ch.Position.Line);
            Assert.AreEqual(9, ch.Position.Column);
            Assert.AreEqual('\n', ch.Value);

            Assert.AreEqual(9, cs.CurrentPosition.Index);
            Assert.AreEqual(2, cs.CurrentPosition.Line);
            Assert.AreEqual(1, cs.CurrentPosition.Column);
            ies = cs.Peek();
            Assert.IsFalse(ies.EndOfInput);
            Assert.IsNotNull(ies.Errors);
            Assert.IsEmpty(ies.Errors);
            Assert.IsNotNull(ies.InputElements);
            Assert.AreEqual(1, ies.InputElements.Count());
            Assert.AreEqual(cs.CurrentPosition, ies.InputElements.First().Position);
            Assert.AreEqual('t', ies.InputElements.First().Value);

            ies = cs.GetNextValue();
            Assert.IsFalse(ies.EndOfInput);
            Assert.IsNotNull(ies.Errors);
            Assert.IsEmpty(ies.Errors);
            Assert.IsNotNull(ies.InputElements);
            Assert.AreEqual(1, ies.InputElements.Count());
            ch = ies.InputElements.First();

            Assert.AreEqual(9, ch.Position.Index);
            Assert.AreEqual(2, ch.Position.Line);
            Assert.AreEqual(1, ch.Position.Column);
            Assert.AreEqual('t', ch.Value);

            ies = cs.GetNextValue();
            Assert.IsFalse(ies.EndOfInput);
            Assert.IsNotNull(ies.Errors);
            Assert.IsEmpty(ies.Errors);
            Assert.IsNotNull(ies.InputElements);
            Assert.AreEqual(1, ies.InputElements.Count());
            ch = ies.InputElements.First();

            Assert.AreEqual(10, ch.Position.Index);
            Assert.AreEqual(2, ch.Position.Line);
            Assert.AreEqual(2, ch.Position.Column);
            Assert.AreEqual('h', ch.Value);

            ies = cs.GetNextValue();
            Assert.IsFalse(ies.EndOfInput);
            Assert.IsNotNull(ies.Errors);
            Assert.IsEmpty(ies.Errors);
            Assert.IsNotNull(ies.InputElements);
            Assert.AreEqual(1, ies.InputElements.Count());
            ch = ies.InputElements.First();

            Assert.AreEqual(11, ch.Position.Index);
            Assert.AreEqual(2, ch.Position.Line);
            Assert.AreEqual(3, ch.Position.Column);
            Assert.AreEqual('r', ch.Value);

            ies = cs.GetNextValue();
            Assert.IsFalse(ies.EndOfInput);
            Assert.IsNotNull(ies.Errors);
            Assert.IsEmpty(ies.Errors);
            Assert.IsNotNull(ies.InputElements);
            Assert.AreEqual(1, ies.InputElements.Count());
            ch = ies.InputElements.First();

            Assert.AreEqual(12, ch.Position.Index);
            Assert.AreEqual(2, ch.Position.Line);
            Assert.AreEqual(4, ch.Position.Column);
            Assert.AreEqual('e', ch.Value);

            Assert.AreEqual(13, cs.CurrentPosition.Index);
            Assert.AreEqual(2, cs.CurrentPosition.Line);
            Assert.AreEqual(5, cs.CurrentPosition.Column);
            ies = cs.Peek();
            Assert.IsFalse(ies.EndOfInput);
            Assert.IsNotNull(ies.Errors);
            Assert.IsEmpty(ies.Errors);
            Assert.IsNotNull(ies.InputElements);
            Assert.AreEqual(1, ies.InputElements.Count());
            Assert.AreEqual(cs.CurrentPosition, ies.InputElements.First().Position);
            Assert.AreEqual('e', ies.InputElements.First().Value);

            ies = cs.GetNextValue();
            Assert.IsFalse(ies.EndOfInput);
            Assert.IsNotNull(ies.Errors);
            Assert.IsEmpty(ies.Errors);
            Assert.IsNotNull(ies.InputElements);
            Assert.AreEqual(1, ies.InputElements.Count());
            ch = ies.InputElements.First();

            Assert.AreEqual(13, ch.Position.Index);
            Assert.AreEqual(2, ch.Position.Line);
            Assert.AreEqual(5, ch.Position.Column);
            Assert.AreEqual('e', ch.Value);

            Assert.AreEqual(14, cs.CurrentPosition.Index);
            Assert.AreEqual(2, cs.CurrentPosition.Line);
            Assert.AreEqual(6, cs.CurrentPosition.Column);
            ies = cs.Peek();
            Assert.IsFalse(ies.EndOfInput);
            Assert.IsNotNull(ies.Errors);
            Assert.IsEmpty(ies.Errors);
            Assert.IsNotNull(ies.InputElements);
            Assert.AreEqual(1, ies.InputElements.Count());
            Assert.AreEqual(cs.CurrentPosition, ies.InputElements.First().Position);
            Assert.AreEqual('\n', ies.InputElements.First().Value);

            Assert.IsFalse(cs.IsAtEnd);

            ies = cs.GetNextValue();
            Assert.IsFalse(ies.EndOfInput);
            Assert.IsNotNull(ies.Errors);
            Assert.IsEmpty(ies.Errors);
            Assert.IsNotNull(ies.InputElements);
            Assert.AreEqual(1, ies.InputElements.Count());
            ch = ies.InputElements.First();

            Assert.AreEqual(14, ch.Position.Index);
            Assert.AreEqual(2, ch.Position.Line);
            Assert.AreEqual(6, ch.Position.Column);
            Assert.AreEqual('\n', ch.Value);

            Assert.AreEqual(15, cs.CurrentPosition.Index);
            Assert.AreEqual(3, cs.CurrentPosition.Line);
            Assert.AreEqual(1, cs.CurrentPosition.Column);

            Assert.IsTrue(cs.IsAtEnd);
        }
    }
}

