using System.Collections.Generic;
using System.Text;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FileSorter.Sorter
{
	public class ByteReader
	{
		private readonly byte eolSymbol = (byte)'\n';
		private readonly byte[] partsSeparator = Encoding.ASCII.GetBytes(". ");

		private readonly byte[] bytes;
		private readonly bool lastBatch;
		private int processedBytes;

		public int ProcessedBytes => processedBytes;

		public ByteReader(byte[] bytes, bool lastBatch)
		{
			this.bytes = bytes;
			this.lastBatch = lastBatch;
		}

		public ICollection<Line> ToLines()
		{
			var timer = System.Diagnostics.Stopwatch.StartNew();
			var adjustedBytes = GetAdjustedBytes();
			processedBytes = adjustedBytes.Length;

			var lines = new List<Line>(adjustedBytes.Length / 512);

			var tasks = Task.Run(() => ParseBytes(lines, adjustedBytes));

			Task.WaitAll(tasks);

			return lines;
		}

		private IEnumerable<Line> ParseBytes(IList<Line> lines, Memory<byte> bytes)
		{
			var partsSeparator = this.partsSeparator.AsSpan();
			int startLineIndex = 0;
			int startStringPartIndex = -1;
			do
			{
				for (int i = startLineIndex; i < bytes.Length; i++)
				{
					if (bytes.Slice(i, partsSeparator.Length).Span.SequenceEqual(partsSeparator))
					{
						startStringPartIndex = i + partsSeparator.Length;
						break;
					}
				}

				if (startStringPartIndex == -1)
				{
					throw new Exception("There is no parts separator symbols.");
				}

				for (int i = startStringPartIndex; i < bytes.Length; i++)
				{
					if (bytes.Span[i] == eolSymbol)
					{
						lines.Add(new Line(bytes.Slice(startLineIndex, i - startLineIndex), startStringPartIndex - startLineIndex));
						startLineIndex = i + 1;
						startStringPartIndex = -1;
						break;
					}
				}

				if (startStringPartIndex != -1)
				{
					lines.Add(new Line(bytes.Slice(startLineIndex, bytes.Length - startLineIndex), startStringPartIndex - startLineIndex));
					startLineIndex = bytes.Length;
					startStringPartIndex = -1;
				}
			} while (startLineIndex < bytes.Length);

			return lines;
		}

		private Memory<byte> GetAdjustedBytes()
		{
			if (lastBatch)
			{
				return RemoveTrailingNewLines();
			}

			for (int i = bytes.Length - 1; i >= 0; i--)
			{
				if (bytes[i] == eolSymbol)
				{
					return bytes.AsMemory(0, i);
				}
			}
			return bytes;
		}

		private Memory<byte> RemoveTrailingNewLines()
		{
			for (int i = bytes.Length - 1; i >= 0; i--)
			{
				if (bytes[i] != eolSymbol)
				{
					return bytes.AsMemory(0, i + 1);
				}
			}
			return bytes;
		}
	}
}
