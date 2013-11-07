using System;
using NUnit.Framework;

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
            Assert.AreEqual('O', cs.Peek().Value);

            var ch = cs.GetNextValue();

            Assert.AreEqual(0, ch.Position.Index);
            Assert.AreEqual(1, ch.Position.Line);
            Assert.AreEqual(1, ch.Position.Column);
            Assert.AreEqual('O', ch.Value);

            Assert.AreEqual(1, cs.CurrentPosition.Index);
            Assert.AreEqual(1, cs.CurrentPosition.Line);
            Assert.AreEqual(2, cs.CurrentPosition.Column);
            Assert.AreEqual('n', cs.Peek().Value);

            ch = cs.GetNextValue();

            Assert.AreEqual(1, ch.Position.Index);
            Assert.AreEqual(1, ch.Position.Line);
            Assert.AreEqual(2, ch.Position.Column);
            Assert.AreEqual('n', ch.Value);

            ch = cs.GetNextValue();

            Assert.AreEqual(2, ch.Position.Index);
            Assert.AreEqual(1, ch.Position.Line);
            Assert.AreEqual(3, ch.Position.Column);
            Assert.AreEqual('e', ch.Value);

            ch = cs.GetNextValue();

            Assert.AreEqual(3, ch.Position.Index);
            Assert.AreEqual(1, ch.Position.Line);
            Assert.AreEqual(4, ch.Position.Column);
            Assert.AreEqual(' ', ch.Value);

            ch = cs.GetNextValue();

            Assert.AreEqual(4, ch.Position.Index);
            Assert.AreEqual(1, ch.Position.Line);
            Assert.AreEqual(5, ch.Position.Column);
            Assert.AreEqual('t', ch.Value);

            ch = cs.GetNextValue();

            Assert.AreEqual(5, ch.Position.Index);
            Assert.AreEqual(1, ch.Position.Line);
            Assert.AreEqual(6, ch.Position.Column);
            Assert.AreEqual('w', ch.Value);

            ch = cs.GetNextValue();

            Assert.AreEqual(6, ch.Position.Index);
            Assert.AreEqual(1, ch.Position.Line);
            Assert.AreEqual(7, ch.Position.Column);
            Assert.AreEqual('o', ch.Value);

            ch = cs.GetNextValue();

            Assert.AreEqual(7, ch.Position.Index);
            Assert.AreEqual(1, ch.Position.Line);
            Assert.AreEqual(8, ch.Position.Column);
            Assert.AreEqual('\r', ch.Value);

            Assert.AreEqual(8, cs.CurrentPosition.Index);
            Assert.AreEqual(1, cs.CurrentPosition.Line);
            Assert.AreEqual(9, cs.CurrentPosition.Column);
            Assert.AreEqual('\n', cs.Peek().Value);

            ch = cs.GetNextValue();

            Assert.AreEqual(8, ch.Position.Index);
            Assert.AreEqual(1, ch.Position.Line);
            Assert.AreEqual(9, ch.Position.Column);
            Assert.AreEqual('\n', ch.Value);

            Assert.AreEqual(9, cs.CurrentPosition.Index);
            Assert.AreEqual(2, cs.CurrentPosition.Line);
            Assert.AreEqual(1, cs.CurrentPosition.Column);
            Assert.AreEqual('t', cs.Peek().Value);

            ch = cs.GetNextValue();

            Assert.AreEqual(9, ch.Position.Index);
            Assert.AreEqual(2, ch.Position.Line);
            Assert.AreEqual(1, ch.Position.Column);
            Assert.AreEqual('t', ch.Value);

            ch = cs.GetNextValue();

            Assert.AreEqual(10, ch.Position.Index);
            Assert.AreEqual(2, ch.Position.Line);
            Assert.AreEqual(2, ch.Position.Column);
            Assert.AreEqual('h', ch.Value);

            ch = cs.GetNextValue();

            Assert.AreEqual(11, ch.Position.Index);
            Assert.AreEqual(2, ch.Position.Line);
            Assert.AreEqual(3, ch.Position.Column);
            Assert.AreEqual('r', ch.Value);

            ch = cs.GetNextValue();

            Assert.AreEqual(12, ch.Position.Index);
            Assert.AreEqual(2, ch.Position.Line);
            Assert.AreEqual(4, ch.Position.Column);
            Assert.AreEqual('e', ch.Value);

            Assert.AreEqual(13, cs.CurrentPosition.Index);
            Assert.AreEqual(2, cs.CurrentPosition.Line);
            Assert.AreEqual(5, cs.CurrentPosition.Column);
            Assert.AreEqual('e', cs.Peek().Value);

            ch = cs.GetNextValue();

            Assert.AreEqual(13, ch.Position.Index);
            Assert.AreEqual(2, ch.Position.Line);
            Assert.AreEqual(5, ch.Position.Column);
            Assert.AreEqual('e', ch.Value);

            Assert.AreEqual(14, cs.CurrentPosition.Index);
            Assert.AreEqual(2, cs.CurrentPosition.Line);
            Assert.AreEqual(6, cs.CurrentPosition.Column);
            Assert.AreEqual('\n', cs.Peek().Value);

            Assert.IsFalse(cs.IsAtEnd);

            ch = cs.GetNextValue();

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

