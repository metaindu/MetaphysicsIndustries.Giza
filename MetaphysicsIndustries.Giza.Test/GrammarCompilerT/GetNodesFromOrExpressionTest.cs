
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
    public class GetNodesFromOrExpressionTest
    {
        [Test]
        public void SingleExprYieldsSimpleNodeBundle()
        {
            // given
            var orexpr = new OrExpression(
                new[]
                {
                    new Expression(
                        new LiteralSubExpression("a"))
                });
            var gc = new GrammarCompiler();
            // precondition
            Assert.IsFalse(orexpr.IsRepeatable);
            Assert.IsFalse(orexpr.IsSkippable);
            // when
            var result = gc.GetNodesFromOrExpression(orexpr, null);
            // then
            Assert.AreEqual(1,result.StartNodes.Count);
            Assert.AreEqual(1,result.EndNodes.Count);
            Assert.AreEqual(1,result.Nodes.Count);
            var node = result.Nodes[0];
            Assert.Contains(node, result.StartNodes.ToList());
            Assert.Contains(node, result.EndNodes.ToList());
            Assert.IsFalse(result.IsSkippable);
        }

        [Test]
        public void TwoExprsYieldTwoMutuallyExclusiveNodes()
        {
            // given
            var orexpr = new OrExpression(
                new[]
                {
                    new Expression(
                        new LiteralSubExpression("a")),
                    new Expression(
                        new LiteralSubExpression("b")),
                });
            var gc = new GrammarCompiler();
            // precondition
            Assert.IsFalse(orexpr.IsRepeatable);
            Assert.IsFalse(orexpr.IsSkippable);
            // when
            var result = gc.GetNodesFromOrExpression(orexpr, null);
            // then
            Assert.AreEqual(2,result.StartNodes.Count);
            Assert.AreEqual(2,result.EndNodes.Count);
            Assert.AreEqual(2,result.Nodes.Count);
            var node0 = result.Nodes[0];
            Assert.Contains(node0, result.StartNodes.ToList());
            Assert.Contains(node0, result.EndNodes.ToList());
            var node1 = result.Nodes[1];
            Assert.Contains(node1, result.StartNodes.ToList());
            Assert.Contains(node1, result.EndNodes.ToList());
            Assert.IsFalse(result.IsSkippable);
        }

        [Test]
        public void TwoSequentialSubExpressionsYieldNotBothStartAndEndNodes()
        {
            // given
            var orexpr = new OrExpression(
                new[]
                {
                    new Expression(
                        new LiteralSubExpression("a"),
                        new LiteralSubExpression("b"))
                });
            var gc = new GrammarCompiler();
            // precondition
            Assert.IsFalse(orexpr.IsRepeatable);
            Assert.IsFalse(orexpr.IsSkippable);
            // when
            var result = gc.GetNodesFromOrExpression(orexpr, null);
            // then
            Assert.AreEqual(1,result.StartNodes.Count);
            Assert.AreEqual(1,result.EndNodes.Count);
            Assert.AreEqual(2,result.Nodes.Count);
            var node0 = result.Nodes[0];
            Assert.Contains(node0, result.StartNodes.ToList());
            Assert.IsFalse(result.EndNodes.Contains(node0));
            var node1 = result.Nodes[1];
            Assert.IsFalse(result.StartNodes.Contains(node1));
            Assert.Contains(node1, result.EndNodes.ToList());
            Assert.IsFalse(result.IsSkippable);
        }

        [Test]
        public void SkippableFirstSubExpressionsYieldsSecondNodeAlsoStartNode()
        {
            // given
            var orexpr = new OrExpression(
                new[]
                {
                    new Expression(
                        new LiteralSubExpression("a", isSkippable: true),
                        new LiteralSubExpression("b"))
                });
            var gc = new GrammarCompiler();
            // precondition
            Assert.IsFalse(orexpr.IsRepeatable);
            Assert.IsFalse(orexpr.IsSkippable);
            // when
            var result = gc.GetNodesFromOrExpression(orexpr, null);
            // then
            Assert.AreEqual(2,result.StartNodes.Count);
            Assert.AreEqual(1,result.EndNodes.Count);
            Assert.AreEqual(2,result.Nodes.Count);
            var node0 = result.Nodes[0];
            Assert.Contains(node0, result.StartNodes.ToList());
            Assert.IsFalse(result.EndNodes.Contains(node0));
            var node1 = result.Nodes[1];
            Assert.Contains(node1, result.StartNodes.ToList());
            Assert.Contains(node1, result.EndNodes.ToList());
            Assert.IsFalse(result.IsSkippable);
        }

        [Test]
        public void SkippableSecondSubExpressionsYieldsFirstNodeAlsoEndNode()
        {
            // given
            var orexpr = new OrExpression(
                // ('a' | 'b'?)
                new[]
                {
                    new Expression(
                        new LiteralSubExpression("a"),
                        new LiteralSubExpression("b", isSkippable: true))
                });
            var gc = new GrammarCompiler();
            // precondition
            Assert.IsFalse(orexpr.IsRepeatable);
            Assert.IsFalse(orexpr.IsSkippable);
            // when
            var result = gc.GetNodesFromOrExpression(orexpr, null);
            // then
            Assert.AreEqual(1,result.StartNodes.Count);
            Assert.AreEqual(2,result.EndNodes.Count);
            Assert.AreEqual(2,result.Nodes.Count);
            var node0 = result.Nodes[0];
            Assert.Contains(node0, result.StartNodes.ToList());
            Assert.Contains(node0, result.EndNodes.ToList());
            var node1 = result.Nodes[1];
            Assert.IsFalse(result.StartNodes.Contains(node1));
            Assert.Contains(node1, result.EndNodes.ToList());
            Assert.IsFalse(result.IsSkippable);
        }
    }
}
