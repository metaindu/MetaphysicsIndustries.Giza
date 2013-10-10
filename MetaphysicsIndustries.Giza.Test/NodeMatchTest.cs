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
            NodeMatch nm = new NodeMatch(node, NodeMatch.TransitionType.Follow, null);
            nm.Index = 123;
            nm.Token = new Token(startIndex: 3, value: "qwer");

            NodeMatch clone = nm.CloneWithNewToken(new Token(startIndex: 5, value: "zxcv"));

            Assert.AreSame(node, clone.Node);
            Assert.AreEqual(NodeMatch.TransitionType.Follow, clone.Transition);
            Assert.AreEqual(123, clone.Index);
            Assert.AreEqual(5, clone.Token.StartIndex);
            Assert.AreEqual("zxcv", clone.Token.Value);
        }
    }
}

