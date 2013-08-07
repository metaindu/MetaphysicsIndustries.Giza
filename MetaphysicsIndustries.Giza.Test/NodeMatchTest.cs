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
            nm.Token = new Token { StartIndex = 3, Length = 3 };

            NodeMatch clone = nm.CloneWithNewToken(new Token { StartIndex = 5, Length = 2 });

            Assert.AreSame(node, clone.Node);
            Assert.AreEqual(NodeMatch.TransitionType.Follow, clone.Transition);
            Assert.AreEqual(123, clone.Index);
            Assert.AreEqual(5, clone.Token.StartIndex);
            Assert.AreEqual(2, clone.Token.Length);
        }
    }
}

