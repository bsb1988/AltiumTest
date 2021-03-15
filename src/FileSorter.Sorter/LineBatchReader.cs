using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;

namespace FileSorter.Sorter
{
	public class LineBatchReader : IEnumerable<IEnumerable<Line>>
	{
		private readonly Stream stream;
		private readonly long batchSize;
		private readonly long startIndex;
		private readonly long length;

		public LineBatchReader(Stream stream, long batchSize)
		{
			this.stream = stream;
			this.batchSize = batchSize;
			startIndex = stream.Position;
			length = stream.Length - stream.Position;
		}

		public LineBatchReader(Stream stream, long batchSize, long startIndex, long length)
		{
			this.stream = stream;
			this.batchSize = batchSize;
			this.startIndex = startIndex;
			this.length = length;
		}

		public IEnumerator<IEnumerable<Line>> GetEnumerator()
		{
			byte[] buffer;
			long processedBytes = 0L;
			bool lastBatch;
			do
			{
				var bytesLeft = length - processedBytes;
				buffer = new byte[Math.Min(batchSize, bytesLeft)];
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
				yield return lines;
			} while (!lastBatch);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
