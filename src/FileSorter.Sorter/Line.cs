using System;
using System.Diagnostics.CodeAnalysis;

namespace FileSorter.Sorter
{
	public class Line : IComparable<Line>
	{
		private readonly Memory<byte> lineBytes;
		private readonly int stringPartStartIndex;

		public Span<byte> LineBytes => lineBytes.Span;

		public Line(Memory<byte> lineBytes, int stringPartStartIndex)
		{
			this.lineBytes = lineBytes;
			this.stringPartStartIndex = stringPartStartIndex;
		}

		public int CompareTo([AllowNull] Line other)
		{
			if (this is null || other is null)
			{
				throw new InvalidOperationException("Both this and other line objects must not have null value.");
			}

			var stringPartCompareResult = lineBytes.Slice(stringPartStartIndex).Span
				.SequenceCompareTo(other.lineBytes.Slice(other.stringPartStartIndex).Span);
			if (stringPartCompareResult != 0)
			{
				return stringPartCompareResult;
			}

			var numberLengthDifference = stringPartStartIndex - other.stringPartStartIndex;

			if (numberLengthDifference == 0)
			{
				return lineBytes.Slice(0, stringPartStartIndex).Span
					.SequenceCompareTo(other.lineBytes.Slice(0, other.stringPartStartIndex).Span);
			}

			return numberLengthDifference;
		}
	}
}
