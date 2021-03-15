using System;
using System.Collections.Generic;

namespace FileSorter.Sorter
{
    public class MinHeap<T> where T: IComparable<T>
	{
		private readonly IList<T> heap = new List<T>();

		public void Add(T item)
		{
			var index = heap.Count;

			heap.Add(item);

			while (index > 0 && heap[index].CompareTo(heap[GetParentIndex(index)]) < 0)
			{
				Swap(index, index = GetParentIndex(index));
			}
		}

		public T GetMin() => heap[0];

		public T ExtractMin()
		{
			if (heap.Count == 0)
			{
				throw new Exception("The heap is empty.");
			}

			var result = heap[0];

			heap[0] = heap[heap.Count - 1];
			heap.RemoveAt(heap.Count - 1);

			Heapify(0);

			return result;
		}

		public int Size => heap.Count;

		private int GetParentIndex(int index) => (index - 1) / 2;
		private int GetLeftChildIndex(int index) => index * 2 + 1;
		private int GetRightChildIndex(int index) => index * 2 + 2;

		private void Swap(int i1, int i2) => (heap[i1], heap[i2]) = (heap[i2], heap[i1]);

		private void Heapify(int index)
		{
			var leftIndex = GetLeftChildIndex(index);
			var rightIndex = GetRightChildIndex(index);
			var smallest = index;

			if (leftIndex < heap.Count && heap[leftIndex].CompareTo(heap[smallest]) < 0)
			{
				smallest = leftIndex;
			}

			if (rightIndex < heap.Count && heap[rightIndex].CompareTo(heap[smallest]) < 0)
			{
				smallest = rightIndex;
			}

			if (smallest != index)
			{
				Swap(smallest, index);
				Heapify(smallest);
			}
		}
	}
}
