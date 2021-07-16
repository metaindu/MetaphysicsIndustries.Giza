﻿
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
            Assert.AreEqual(1, pq.Count);
        }

        [Test]
        public void TestPeek()
        {
            // setup
            var pq = new PriorityQueue<string, int>();

            // action
            pq.Enqueue("something2", 4);

            // assertion
            Assert.AreEqual(1, pq.Count);
            Assert.AreEqual("something2", pq.Peek());
        }

        [Test]
        public void TestRemove()
        {
            // setup
            var pq = new PriorityQueue<string, int>();
            pq.Enqueue("item", 2);
            Assert.AreEqual(1, pq.Count);
            Assert.AreEqual("item", pq.Peek());

            // action
            string value = pq.Dequeue();

            // assertions
            Assert.AreEqual(0, pq.Count);
            Assert.AreSame("item", value);
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
            Assert.AreEqual(2, pq.Count);
            Assert.AreEqual("something else", pq.Peek());
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
            Assert.AreEqual(2, pq.Count);
            Assert.AreEqual("high", pq.Peek());
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
            Assert.AreEqual("high", array[0]);
            Assert.AreEqual("middle", array[1]);
            Assert.AreEqual("low", array[2]);
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
            Assert.AreEqual("high", array[0]);
            Assert.AreEqual("middle", array[1]);
            Assert.AreEqual("low", array[2]);
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
            Assert.AreEqual("high", array[0]);
            Assert.AreEqual("high2", array[1]);
            Assert.AreEqual("low", array[2]);
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
            Assert.AreEqual("high", array[0]);
            Assert.AreEqual("low", array[1]);
            Assert.AreEqual("low2", array[2]);
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
            Assert.AreEqual("low", array[0]);
            Assert.AreEqual("high", array[1]);
            Assert.AreEqual("high2", array[2]);
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
            Assert.AreEqual("low", array[0]);
            Assert.AreEqual("low2", array[1]);
            Assert.AreEqual("high", array[2]);
        }
    }
}

