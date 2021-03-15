using FileSorter.Generator.Interfaces;
using System;

namespace FileSorter.Generator
{
	public class DuplicatePartGenerator : ILineGenerator
	{
		private readonly ILineGenerator lineGenerator;
		private readonly IPartGenerator numberPartGenerator;
		private readonly IRandomGenerator randomGenerator;
		private readonly ILineGeneratorSettings settings;
		private readonly int uniqueValuesPerDuplicate;
		private readonly byte[] result;

		private long generatedUniqueLinesCount;
		private Memory<byte> stringPartToDuplicate;

		public DuplicatePartGenerator(
			ILineGenerator lineGenerator,
			IPartGenerator numberPartGenerator,
			IRandomGenerator randomGenerator,
			ILineGeneratorSettings lineSettings,
			int uniqueValuesPerDuplicate)
		{
			this.lineGenerator = lineGenerator;
			this.numberPartGenerator = numberPartGenerator;
			this.randomGenerator = randomGenerator;
			this.settings = lineSettings;
			this.uniqueValuesPerDuplicate = uniqueValuesPerDuplicate;
			result = new byte[lineSettings.MaxLineLength];
		}

		public LineInfo GetNewLine()
		{
			if (generatedUniqueLinesCount == uniqueValuesPerDuplicate)
			{
				return GetDuplicatedLine();
			}

			var line = lineGenerator.GetNewLine();

			if (generatedUniqueLinesCount == 0)
			{
				stringPartToDuplicate = line.StringPart.ToArray();
			}

			generatedUniqueLinesCount++;

			return line;
		}

		private LineInfo GetDuplicatedLine()
		{
			generatedUniqueLinesCount = 0;

			var numberPartLength = randomGenerator.Next(
				settings.MinNumberLength,
				Math.Min(
					settings.MaxNumberLength,
					settings.MaxLineLength - stringPartToDuplicate.Length - settings.PartsSeparatorBytes.Length));

			var stringPartStartIndex = numberPartLength + settings.PartsSeparatorBytes.Length;
			var stringLength = stringPartStartIndex + stringPartToDuplicate.Length;

			var rndBytes = new byte[numberPartLength];
			randomGenerator.NextBytes(rndBytes, 0, rndBytes.Length);

			numberPartGenerator.Fill(result, 0, rndBytes);
			settings.PartsSeparatorBytes.CopyTo(result, numberPartLength);
			stringPartToDuplicate.CopyTo(result.AsMemory(stringPartStartIndex, stringPartToDuplicate.Length));

			return
				new LineInfo(
					result.AsSpan(0, stringLength),
					result.AsSpan(0, numberPartLength),
					result.AsSpan(stringPartStartIndex, stringPartToDuplicate.Length));
		}
	}
}
