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
    [TestFixture]
    public class PriorityQueueTest
    {
        [Test]
        public void TestAddToEmpty()
        {
            // setup
            var pq = new PriorityQueue<string, int>();

            // action
            pq.Enqueue("something", 3);

            // assertion
            Assert.That(pq.Count, Is.EqualTo(1));
        }

        [Test]
        public void TestPeek()
        {
            // setup
            var pq = new PriorityQueue<string, int>();

            // action
            pq.Enqueue("something2", 4);

            // assertion
            Assert.That(pq.Count, Is.EqualTo(1));
            Assert.That(pq.Peek(), Is.EqualTo("something2"));
        }

        [Test]
        public void TestRemove()
        {
            // setup
            var pq = new PriorityQueue<string, int>();
            pq.Enqueue("item", 2);
            Assert.That(pq.Count, Is.EqualTo(1));
            Assert.That(pq.Peek(), Is.EqualTo("item"));

            // action
            string value = pq.Dequeue();

            // assertions
            Assert.That(pq.Count, Is.EqualTo(0));
            Assert.That(value, Is.SameAs("item"));
        }

        [Test]
        public void TestNormalUsage()
        {
        }

        [Test]
        public void TestAddToFront()
        {
            // setup
            var pq = new PriorityQueue<string, int>();
            pq.Enqueue("something", 1);

            // action
            pq.Enqueue("something else", 5);

            // assertion
            Assert.That(pq.Count, Is.EqualTo(2));
            Assert.That(pq.Peek(), Is.EqualTo("something else"));
        }

        [Test]
        public void TestAddToBack()
        {
            // setup
            var pq = new PriorityQueue<string, int>();
            pq.Enqueue("high", 5);

            // action
            pq.Enqueue("low", 1);

            // assertion
            Assert.That(pq.Count, Is.EqualTo(2));
            Assert.That(pq.Peek(), Is.EqualTo("high"));
        }

        [Test]
        public void TestCopyTo()
        {
            // setup
            var pq = new PriorityQueue<string, int>();
            pq.Enqueue("high", 5);
            pq.Enqueue("middle", 3);
            pq.Enqueue("low", 1);
            var array = new string[3];

            // action
            pq.CopyTo(array, 0);

            // assertion
            Assert.That(array[0], Is.EqualTo("high"));
            Assert.That(array[1], Is.EqualTo("middle"));
            Assert.That(array[2], Is.EqualTo("low"));
        }

        [Test]
        public void TestAddToMiddle()
        {
            // setup
            var pq = new PriorityQueue<string, int>();
            pq.Enqueue("high", 5);
            pq.Enqueue("low", 1);
            var array = new string[3];

            // action
            pq.Enqueue("middle", 3);
            pq.CopyTo(array, 0);

            // assertion
            Assert.That(array[0], Is.EqualTo("high"));
            Assert.That(array[1], Is.EqualTo("middle"));
            Assert.That(array[2], Is.EqualTo("low"));
        }

        [Test]
        public void TestAddSamePriorityHigh()
        {
            // setup
            var pq = new PriorityQueue<string, int>();
            pq.Enqueue("high", 5);
            pq.Enqueue("low", 1);
            var array = new string[3];

            // action
            pq.Enqueue("high2", 5);
            pq.CopyTo(array, 0);

            // assertion
            Assert.That(array[0], Is.EqualTo("high"));
            Assert.That(array[1], Is.EqualTo("high2"));
            Assert.That(array[2], Is.EqualTo("low"));
        }

        [Test]
        public void TestAddSamePriorityLow()
        {
            // setup
            var pq = new PriorityQueue<string, int>();
            pq.Enqueue("high", 5);
            pq.Enqueue("low", 1);
            var array = new string[3];

            // action
            pq.Enqueue("low2", 1);
            pq.CopyTo(array, 0);

            // assertion
            Assert.That(array[0], Is.EqualTo("high"));
            Assert.That(array[1], Is.EqualTo("low"));
            Assert.That(array[2], Is.EqualTo("low2"));
        }

        [Test]
        public void TestLowToHighAddSamePriorityHigh()
        {
            // setup
            var pq = new PriorityQueue<string, int>(lowToHigh: true);
            pq.Enqueue("high", 5);
            pq.Enqueue("low", 1);
            var array = new string[3];

            // action
            pq.Enqueue("high2", 5);
            pq.CopyTo(array, 0);

            // assertion
            Assert.That(array[0], Is.EqualTo("low"));
            Assert.That(array[1], Is.EqualTo("high"));
            Assert.That(array[2], Is.EqualTo("high2"));
        }

        [Test]
        public void TestLowToHighAddSamePriorityLow()
        {
            // setup
            var pq = new PriorityQueue<string, int>(lowToHigh: true);
            pq.Enqueue("high", 5);
            pq.Enqueue("low", 1);
            var array = new string[3];

            // action
            pq.Enqueue("low2", 1);
            pq.CopyTo(array, 0);

            // assertion
            Assert.That(array[0], Is.EqualTo("low"));
            Assert.That(array[1], Is.EqualTo("low2"));
            Assert.That(array[2], Is.EqualTo("high"));
        }
    }
}