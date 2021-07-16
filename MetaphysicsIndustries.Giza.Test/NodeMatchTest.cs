
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
    public class NodeMatchTest
    {
        [Test]
        public void TestNodeMatchClone()
        {
            Node node = new CharNode('c', "asdf");
            var nm = new NodeMatch<Token>(node, TransitionType.Follow, null);
            nm.AlternateStartPosition.Index = 123;
            nm.InputElement = new Token(startPosition: new InputPosition(3), value: "qwer");

            var clone = nm.CloneWithNewInputElement(new Token(startPosition: new InputPosition(5), value: "zxcv"));

            Assert.AreSame(node, clone.Node);
            Assert.AreEqual(TransitionType.Follow, clone.Transition);
            Assert.AreEqual(5, clone.StartPosition.Index);
            Assert.AreEqual(5, clone.InputElement.StartPosition.Index);
            Assert.AreEqual("zxcv", clone.InputElement.Value);
        }
    }
}

