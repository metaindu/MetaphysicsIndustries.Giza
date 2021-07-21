
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

using System.Linq;
using NUnit.Framework;

namespace MetaphysicsIndustries.Giza.Test.GrammarCompilerT
{
    [TestFixture]
    public class GetNodesFromExpressionTest
    {
        [Test]
        public void SingleItemYieldsSimpleNode()
        {
            // given
            var expr = new Expression(
                new LiteralSubExpression("a"));
            var gc = new GrammarCompiler();
            // when
            var result = gc.GetNodesFromExpression(expr, null);
            // then
            Assert.IsFalse(result.IsSkippable);
            Assert.AreEqual(1, result.Nodes.Count);
            Assert.AreEqual(1, result.StartNodes.Count);
            Assert.AreEqual(1, result.EndNodes.Count);
            var node = result.Nodes[0];
            Assert.Contains(node, result.StartNodes.ToList());
            Assert.Contains(node, result.EndNodes.ToList());
            Assert.IsEmpty(node.NextNodes);
            Assert.IsFalse(result.IsSkippable);
        }

        [Test]
        public void MultipleItemsYieldMultipleNodes()
        {
            // given
            var expr = new Expression(
                new LiteralSubExpression("a"),
                new LiteralSubExpression("b"));
            var gc = new GrammarCompiler();
            // when
            var result = gc.GetNodesFromExpression(expr, null);
            // then
            Assert.IsFalse(result.IsSkippable);
            Assert.AreEqual(2, result.Nodes.Count);
            Assert.AreEqual(1, result.StartNodes.Count);
            Assert.AreEqual(1, result.EndNodes.Count);
            var node0 = result.Nodes[0];
            var node1 = result.Nodes[1];
            Assert.Contains(node0, result.StartNodes.ToList());
            Assert.Contains(node1, result.EndNodes.ToList());
            Assert.AreEqual(1, node0.NextNodes.Count);
            Assert.Contains(node1, node0.NextNodes.ToList());
            Assert.IsEmpty(node1.NextNodes);
            Assert.IsFalse(result.IsSkippable);
        }

        [Test]
        public void SkippableFirstItemYieldsSecondNodeAlsoStartNode()
        {
            // given
            var expr = new Expression(
                new LiteralSubExpression("a", isSkippable: true),
                new LiteralSubExpression("b"));
            var gc = new GrammarCompiler();
            // when
            var result = gc.GetNodesFromExpression(expr, null);
            // then
            Assert.IsFalse(result.IsSkippable);
            Assert.AreEqual(2,result.Nodes.Count);
            Assert.AreEqual(2,result.StartNodes.Count);
            Assert.AreEqual(1,result.EndNodes.Count);
            var node0 = result.Nodes[0];
            Assert.Contains(node0, result.StartNodes.ToList());
            Assert.IsFalse(result.EndNodes.Contains(node0));
            var node1 = result.Nodes[1];
            Assert.Contains(node1, result.StartNodes.ToList());
            Assert.Contains(node1, result.EndNodes.ToList());
            Assert.IsFalse(result.IsSkippable);
        }

        [Test]
        public void SkippableSecondItemYieldsFirstNodeAlsoEndNode()
        {
            // given
            var expr = new Expression(
                new LiteralSubExpression("a"),
                new LiteralSubExpression("b", isSkippable: true));
            var gc = new GrammarCompiler();
            // when
            var result = gc.GetNodesFromExpression(expr, null);
            // then
            Assert.IsFalse(result.IsSkippable);
            Assert.AreEqual(2,result.Nodes.Count);
            Assert.AreEqual(1,result.StartNodes.Count);
            Assert.AreEqual(2,result.EndNodes.Count);
            var node0 = result.Nodes[0];
            Assert.Contains(node0, result.StartNodes.ToList());
            Assert.Contains(node0, result.EndNodes.ToList());
            var node1 = result.Nodes[1];
            Assert.IsFalse(result.StartNodes.Contains(node1));
            Assert.Contains(node1, result.EndNodes.ToList());
            Assert.IsFalse(result.IsSkippable);
        }

        [Test]
        public void NonSubsequentSkippableItemsYieldsSimpleGraph()
        {
            // given
            var expr = new Expression(
                new LiteralSubExpression("a"),
                new LiteralSubExpression("b", isSkippable: true),
                new LiteralSubExpression("c"),
                new LiteralSubExpression("d", isSkippable: true),
                new LiteralSubExpression("e"));
            var gc = new GrammarCompiler();
            // when
            var result = gc.GetNodesFromExpression(expr, null);
            // then
            Assert.IsFalse(result.IsSkippable);
            Assert.AreEqual(5,result.Nodes.Count);
            Assert.AreEqual(1,result.StartNodes.Count);
            Assert.AreEqual(1,result.EndNodes.Count);

            var node0 = result.Nodes[0];
            var node1 = result.Nodes[1];
            var node2 = result.Nodes[2];
            var node3 = result.Nodes[3];
            var node4 = result.Nodes[4];

            Assert.IsTrue(result.StartNodes.Contains(node0));
            Assert.IsFalse(result.EndNodes.Contains(node0));
            Assert.AreEqual(2, node0.NextNodes.Count);
            Assert.Contains(node1,node0.NextNodes.ToList());
            Assert.Contains(node2,node0.NextNodes.ToList());

            Assert.IsFalse(result.StartNodes.Contains(node1));
            Assert.IsFalse(result.EndNodes.Contains(node1));
            Assert.AreEqual(1, node1.NextNodes.Count);
            Assert.Contains(node2,node1.NextNodes.ToList());

            Assert.IsFalse(result.StartNodes.Contains(node2));
            Assert.IsFalse(result.EndNodes.Contains(node2));
            Assert.AreEqual(2, node2.NextNodes.Count);
            Assert.Contains(node3,node2.NextNodes.ToList());
            Assert.Contains(node4,node2.NextNodes.ToList());

            Assert.IsFalse(result.StartNodes.Contains(node3));
            Assert.IsFalse(result.EndNodes.Contains(node3));
            Assert.AreEqual(1, node3.NextNodes.Count);
            Assert.Contains(node4,node3.NextNodes.ToList());

            Assert.IsFalse(result.StartNodes.Contains(node4));
            Assert.IsTrue(result.EndNodes.Contains(node4));
            Assert.AreEqual(0, node4.NextNodes.Count);
        }

        [Test]
        public void SubsequentSkippableItemsYieldsComplexGraph()
        {
            // given
            var expr = new Expression(
                new LiteralSubExpression("a"),
                new LiteralSubExpression("b", isSkippable: true),
                new LiteralSubExpression("c", isSkippable: true),
                new LiteralSubExpression("d", isSkippable: true),
                new LiteralSubExpression("e"));
            var gc = new GrammarCompiler();
            // when
            var result = gc.GetNodesFromExpression(expr, null);
            // then
            Assert.IsFalse(result.IsSkippable);
            Assert.AreEqual(5,result.Nodes.Count);
            Assert.AreEqual(1,result.StartNodes.Count);
            Assert.AreEqual(1,result.EndNodes.Count);

            var node0 = result.Nodes[0];
            var node1 = result.Nodes[1];
            var node2 = result.Nodes[2];
            var node3 = result.Nodes[3];
            var node4 = result.Nodes[4];

            Assert.IsTrue(result.StartNodes.Contains(node0));
            Assert.IsFalse(result.EndNodes.Contains(node0));
            Assert.AreEqual(4, node0.NextNodes.Count);
            Assert.Contains(node1,node0.NextNodes.ToList());
            Assert.Contains(node2,node0.NextNodes.ToList());
            Assert.Contains(node3,node0.NextNodes.ToList());
            Assert.Contains(node4,node0.NextNodes.ToList());

            Assert.IsFalse(result.StartNodes.Contains(node1));
            Assert.IsFalse(result.EndNodes.Contains(node1));
            Assert.AreEqual(3, node1.NextNodes.Count);
            Assert.Contains(node2,node1.NextNodes.ToList());
            Assert.Contains(node3,node1.NextNodes.ToList());
            Assert.Contains(node4,node1.NextNodes.ToList());

            Assert.IsFalse(result.StartNodes.Contains(node2));
            Assert.IsFalse(result.EndNodes.Contains(node2));
            Assert.AreEqual(2, node2.NextNodes.Count);
            Assert.Contains(node3,node2.NextNodes.ToList());
            Assert.Contains(node4,node2.NextNodes.ToList());

            Assert.IsFalse(result.StartNodes.Contains(node3));
            Assert.IsFalse(result.EndNodes.Contains(node3));
            Assert.AreEqual(1, node3.NextNodes.Count);
            Assert.Contains(node4,node3.NextNodes.ToList());

            Assert.IsFalse(result.StartNodes.Contains(node4));
            Assert.IsTrue(result.EndNodes.Contains(node4));
            Assert.AreEqual(0, node4.NextNodes.Count);
        }

        [Test]
        public void SubsequentSkippableItemsYieldsComplexGraph2()
        {
            // given
            var expr = new Expression(
                new LiteralSubExpression("a"),
                new LiteralSubExpression("b", isSkippable: true),
                new LiteralSubExpression("c", isSkippable: true),
                new LiteralSubExpression("d"),
                new LiteralSubExpression("e"));
            var gc = new GrammarCompiler();
            // when
            var result = gc.GetNodesFromExpression(expr, null);
            // then
            Assert.IsFalse(result.IsSkippable);
            Assert.AreEqual(5,result.Nodes.Count);
            Assert.AreEqual(1,result.StartNodes.Count);
            Assert.AreEqual(1,result.EndNodes.Count);

            var node0 = result.Nodes[0];
            var node1 = result.Nodes[1];
            var node2 = result.Nodes[2];
            var node3 = result.Nodes[3];
            var node4 = result.Nodes[4];

            Assert.IsTrue(result.StartNodes.Contains(node0));
            Assert.IsFalse(result.EndNodes.Contains(node0));
            Assert.AreEqual(3, node0.NextNodes.Count);
            Assert.Contains(node1,node0.NextNodes.ToList());
            Assert.Contains(node2,node0.NextNodes.ToList());
            Assert.Contains(node3,node0.NextNodes.ToList());

            Assert.IsFalse(result.StartNodes.Contains(node1));
            Assert.IsFalse(result.EndNodes.Contains(node1));
            Assert.AreEqual(2, node1.NextNodes.Count);
            Assert.Contains(node2,node1.NextNodes.ToList());
            Assert.Contains(node3,node1.NextNodes.ToList());

            Assert.IsFalse(result.StartNodes.Contains(node2));
            Assert.IsFalse(result.EndNodes.Contains(node2));
            Assert.AreEqual(1, node2.NextNodes.Count);
            Assert.Contains(node3,node2.NextNodes.ToList());

            Assert.IsFalse(result.StartNodes.Contains(node3));
            Assert.IsFalse(result.EndNodes.Contains(node3));
            Assert.AreEqual(1, node3.NextNodes.Count);
            Assert.Contains(node4,node3.NextNodes.ToList());

            Assert.IsFalse(result.StartNodes.Contains(node4));
            Assert.IsTrue(result.EndNodes.Contains(node4));
            Assert.AreEqual(0, node4.NextNodes.Count);
        }
    }
}
