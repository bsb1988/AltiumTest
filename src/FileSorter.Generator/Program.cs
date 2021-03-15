using System;
using System.Diagnostics;
using System.IO;
using System.Timers;

namespace FileSorter.Generator
{
	class Program
	{
		private static int bufferSize = 16384;
		private static Stopwatch stopwatch;

		static void Main(string[] args)
		{
			if (args.Length == 0 || args[0] == "/?")
			{
				Console.WriteLine("Usage: FileSorter.Generator.exe target_file_path size_MB [MinNumberPartLength:MaxNumberPartLength:MinLineLength:MaxLineLength:DuplicateNthStringPart]\n\nExample:\nFileSorter.Generator.exe d:\\file.txt 10240 1:10:15:1024:100");
				return;
			}

			var targetFilePath = args[0];

			var targetFileInfo = new FileInfo(targetFilePath);

			ulong fileSizeMB = ulong.Parse(args[1]);
			ulong fileSizeBytes = fileSizeMB * (1 << 20);

			var driveInfo = new DriveInfo(targetFileInfo.Directory.Root.Name);

			if (driveInfo.AvailableFreeSpace < ((long)fileSizeBytes - (targetFileInfo.Exists ? targetFileInfo.Length : 0)))
			{
				Console.WriteLine($"You do need {((long)fileSizeBytes - (targetFileInfo.Exists ? targetFileInfo.Length : 0) - driveInfo.AvailableFreeSpace) >> 20} MB free space more to generate test file.");
				return;
			}

			ushort minNumberLength = 1;
			ushort maxNumberLength = 10;
			ushort minLineLength = 15;
			ushort maxLineLength = 1024;
			string partsSeparator = ". ";
			ushort uniqueValuesPerDuplicate = 100;

			if (args.Length > 2)
			{
				var settingsParts = args[2].Split(':');
				minNumberLength = ushort.Parse(settingsParts[0]);
				maxNumberLength = ushort.Parse(settingsParts[1]);
				minLineLength = ushort.Parse(settingsParts[2]);
				maxLineLength = ushort.Parse(settingsParts[3]);
				uniqueValuesPerDuplicate = ushort.Parse(settingsParts[4]);
			}
			var lineSettings = new LineGeneratorSettings(
				minNumberLength,
				maxNumberLength,
				minLineLength,
				maxLineLength,
				partsSeparator);

			Console.WriteLine($"Start generating \"{targetFilePath}\" {fileSizeMB}MB...");

			stopwatch = Stopwatch.StartNew();
			var numberPartGenerator = new NumberPartGenerator();
			var randomGenerator = new FastRandomGenerator();

			using (var timer = new Timer(5000))
			{
				Console.Write($"Elapsed: {stopwatch.Elapsed.ToString(@"hh\:mm\:ss")}...");
				timer.Elapsed += Timer_Elapsed;
				timer.Start();

				using (var fileStream = new FileStream(
					targetFilePath,
					FileMode.Create,
					FileAccess.Write,
					FileShare.None,
					bufferSize,
					false))
				{
					var dataGenerator = new TestFileGenerator(
						new FileSettings(fileSizeBytes, "\n"),
						new DuplicatePartGenerator(
							new LineGenerator(
								randomGenerator,
								lineSettings,
								numberPartGenerator,
								new StringPartGenerator()),
							numberPartGenerator,
							randomGenerator,
							lineSettings,
							uniqueValuesPerDuplicate),
						fileStream);

					dataGenerator.Generate();
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
