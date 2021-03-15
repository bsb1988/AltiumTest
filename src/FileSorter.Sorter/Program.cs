using System;
using System.Diagnostics;
using System.IO;
using System.Timers;

namespace FileSorter.Sorter
{
	public class Program
	{
		private static int bufferSize = 16384;
		// It looks like 500MB is an optimal value;
		private static long maxBatchSize = 500 * (1 << 20);

		private static Stopwatch stopwatch;

		static void Main(string[] args)
		{
			if (args.Length == 0 || args[0] == "/?")
			{
				Console.WriteLine("Usage: FileSorter.Sorter.exe source_file [target_file]");
				return;
			}

			var srcFilePath = args[0];

			var srcFileInfo = new FileInfo(srcFilePath);
			if (!srcFileInfo.Exists)
			{
				Console.WriteLine($"The source file \"{srcFilePath}\" does not exists.");
				return;
			}

			string targetFilePath;
			if (args.Length > 1)
			{
				targetFilePath = args[1];
			}
			else
			{
				targetFilePath = Path.Combine(
					Path.GetDirectoryName(srcFilePath),
					$"{Path.GetFileNameWithoutExtension(srcFilePath)}_sorted{Path.GetExtension(srcFilePath)}");
			}

			var targetFileInfo = new FileInfo(targetFilePath);

			var tempFile = $"{targetFilePath}.temp";
			var tempFileInfo = new FileInfo(tempFile);
			if (targetFileInfo.Exists)
			{
				targetFileInfo.Delete();
			}
			if (tempFileInfo.Exists)
			{
				tempFileInfo.Delete();
			}

			var driveInfo = new DriveInfo(targetFileInfo.Directory.Root.Name);

			if (driveInfo.AvailableFreeSpace < (srcFileInfo.Length * 2))
			{
				Console.WriteLine($"You do need {(srcFileInfo.Length * 2 - driveInfo.AvailableFreeSpace) >> 20} MB free space more to sort the selected file.");
				return;
			}

			Console.WriteLine($"Start sorting \"{srcFileInfo}\" into \"{targetFilePath}\"...");

			stopwatch = Stopwatch.StartNew();

			using (var timer = new Timer(5000))
			{
				Console.Write($"Elapsed: {stopwatch.Elapsed.ToString(@"hh\:mm\:ss")}...");
				timer.Elapsed += Timer_Elapsed;
				timer.Start();

				using (var srcStream = new FileStream(srcFilePath, FileMode.Open, FileAccess.Read, FileShare.None, bufferSize, false))
				{
					var sorter = new LargeFileSorter(new LineBatchReader(srcStream, maxBatchSize), targetFilePath, tempFile);
					sorter.Run();
				}

				timer.Stop();
				timer.Elapsed -= Timer_Elapsed;

				Console.WriteLine($"\n\nCompleted in {stopwatch.Elapsed}.");
			}
		}

		private static void Timer_Elapsed(object sender, ElapsedEventArgs e)
		{
			Console.SetCursorPosition(0, Console.GetCursorPosition().Top);
			Console.Write($"Elapsed: {stopwatch.Elapsed.ToString(@"hh\:mm\:ss")}...");
		}
	}
}
