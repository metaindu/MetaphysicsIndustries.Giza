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
            var nm = new NodeMatch<Token>(node, NodeMatch<Token>.TransitionType.Follow, null);
            nm.AlternateStartPosition.Index = 123;
            nm.Token = new Token(startPosition: new InputPosition(3), value: "qwer");

            var clone = nm.CloneWithNewToken(new Token(startPosition: new InputPosition(5), value: "zxcv"));

            Assert.AreSame(node, clone.Node);
            Assert.AreEqual(NodeMatch<Token>.TransitionType.Follow, clone.Transition);
            Assert.AreEqual(5, clone.StartPosition.Index);
            Assert.AreEqual(5, clone.Token.StartPosition.Index);
            Assert.AreEqual("zxcv", clone.Token.Value);
        }
    }
}

