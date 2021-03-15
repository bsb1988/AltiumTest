using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace FileSorter.Sorter.Tests
{
	[TestFixture]
	public class MinHeapTests
	{
		[Test]
		public void Test()
		{
			var minHeap = new MinHeap<int>();

			for (int i = 10; i >= 0; i--)
			{
				minHeap.Add(i);
				Assert.That(minHeap.GetMin(), Is.EqualTo(i));
			}

			for (int i = 0; i <= 10; i++)
			{
				Assert.That(minHeap.ExtractMin(), Is.EqualTo(i));
			}
		}
	}
}
