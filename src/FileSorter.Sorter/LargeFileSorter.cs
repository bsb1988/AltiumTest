using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FileSorter.Sorter
{
	public class LargeFileSorter
	{
		private static int bufferSize = 16384;

		private readonly string targetFilePath;
		private readonly string tempFilePath;
		private readonly IEnumerable<IEnumerable<Line>> batchReader;

		public LargeFileSorter(
			IEnumerable<IEnumerable<Line>> batchReader,
			string targetFilePath,
			string tempFilePath)
		{
			this.batchReader = batchReader;
			this.targetFilePath = targetFilePath;
			this.tempFilePath = tempFilePath;
		}

		public void Run()
		{
			SortBatches();
			MergeBatches();
		}

		private List<(long StartIndex, long Length)> startBatchesPositions = new List<(long StartIndex, long Length)>();

		private void SortBatches()
		{
			using (var tempFile = new FileStream(
				tempFilePath,
				FileMode.Create,
				FileAccess.Write,
				FileShare.None,
				bufferSize,
				false))
			{
				var enumerator = batchReader.GetEnumerator();
				enumerator.MoveNext();

				var orderLinesTask = Task.Run(() => OrderLines(enumerator.Current));

				while (enumerator.MoveNext())
				{
					var linesToSave = orderLinesTask.Result;

					orderLinesTask = Task.Run(() => OrderLines(enumerator.Current));

					SaveBatch(tempFile, linesToSave);
				}

				SaveBatch(tempFile, orderLinesTask.Result);
			}
		}

		private IList<Line> OrderLines(IEnumerable<Line> lines)
		{
			return lines
				.Select((l, i) => new { Line = l, Index = i })
				.AsParallel()
				.OrderBy(x => x.Line)
				.ThenBy(x => x.Index)
				.Select(x => x.Line)
				.ToList();
		}

		private (long StartIndex, long Length) SaveLines(Stream fileStream, IEnumerable<Line> linesToSave)
		{
			var startIndex = fileStream.Position;

			var enumerator = linesToSave.GetEnumerator();
			enumerator.MoveNext();
			fileStream.Write(enumerator.Current.LineBytes);
			while (enumerator.MoveNext())
			{
				fileStream.WriteByte((byte)'\n');
				fileStream.Write(enumerator.Current.LineBytes);
			}
			return (startIndex, fileStream.Position - startIndex);
		}

		private void SaveBatch(Stream fileStream, IEnumerable<Line> linesToSave)
		{
			startBatchesPositions.Add(SaveLines(fileStream, linesToSave));
		}

		private void MergeBatches()
		{
			using (var tempFile = new FileStream(
				tempFilePath,
				FileMode.Open,
				FileAccess.Read,
				FileShare.None,
				bufferSize,
				false))
			{
				var readers = startBatchesPositions
					.Select(bp => new LineReader(
						tempFile,
						bp.Length / startBatchesPositions.Count,
						bp.StartIndex,
						bp.Length))
					.ToList();

				var mergeSorter = new MergeSorterMinHeap<Line>(readers);

				using (var fileStream = new FileStream(
						targetFilePath,
						FileMode.Create,
						FileAccess.Write,
						FileShare.None,
						bufferSize,
						false))
				{
					SaveLines(fileStream, mergeSorter);
				}
			}

			File.Delete(tempFilePath);
		}
	}
}
