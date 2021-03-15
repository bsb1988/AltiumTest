using System.IO;
using System.Collections.Generic;
using System.Collections;
using System;

namespace FileSorter.Sorter
{
	public class LineReader : IEnumerable<Line>
	{
		private readonly Stream stream;
		private readonly long bufferSize;
		private readonly long startIndex;
		private readonly long length;

		public LineReader(Stream stream, long bufferSize)
			: this(stream, bufferSize, stream.Position, stream.Length - stream.Position)
		{
		}

		public LineReader(Stream stream, long bufferSize, long startIndex, long length)
		{
			this.stream = stream;
			this.bufferSize = bufferSize;
			this.startIndex = startIndex;
			this.length = length;
		}

		public IEnumerator<Line> GetEnumerator()
		{
			byte[] buffer;
			long processedBytes = 0;
			bool lastBatch;
			do
			{
				var bytesLeft = length - processedBytes;
				buffer = new byte[Math.Min(bufferSize, bytesLeft)];
				lastBatch = buffer.Length == bytesLeft;

				stream.Seek(startIndex + processedBytes, SeekOrigin.Begin);

				stream.Read(buffer, 0, buffer.Length);

				var byteReader = new ByteReader(buffer, lastBatch);
				var lines = byteReader.ToLines();

				if (lastBatch)
				{
					processedBytes += buffer.Length;
				}
				else
				{
					processedBytes += byteReader.ProcessedBytes + 1;
				}

				foreach (var line in lines)
				{
					yield return line;
				}
			} while (!lastBatch);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
