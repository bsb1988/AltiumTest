using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Diagnostics.CodeAnalysis;
using System;

namespace FileSorter.Sorter
{
	public class MergeSorterMinHeap<T>: IEnumerable<T> where T : IComparable<T>
	{
		private class MergeSorterCollectionWrapper<TItem> : IComparable<MergeSorterCollectionWrapper<TItem>> where TItem : IComparable<TItem>
		{
			private readonly IEnumerator<TItem> items;

			public MergeSorterCollectionWrapper(IEnumerable<TItem> items)
			{
				this.items = items.GetEnumerator();
				this.items.MoveNext();
			}

			public bool MoveNext() => items.MoveNext();
			public TItem Current => items.Current;

			public int CompareTo([AllowNull] MergeSorterCollectionWrapper<TItem> other) => items.Current.CompareTo(other.items.Current);
		}

		private readonly IEnumerable<IEnumerable<T>> collections;

		public MergeSorterMinHeap(IEnumerable<IEnumerable<T>> collections)
		{
			this.collections = collections;
		}

		public IEnumerator<T> GetEnumerator()
		{
			var minHeap = new MinHeap<MergeSorterCollectionWrapper<T>>();

			foreach (var item in collections.Select(lr => new MergeSorterCollectionWrapper<T>(lr)))
			{
				minHeap.Add(item);
			}

			while (minHeap.Size > 0)
			{
				var min = minHeap.ExtractMin();
				yield return min.Current;
				if (min.MoveNext())
				{
					minHeap.Add(min);
				}
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
