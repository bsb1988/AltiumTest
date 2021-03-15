using FileSorter.Generator.Interfaces;
using System;

namespace FileSorter.Generator
{
	public class LineGenerator : ILineGenerator
	{
		private readonly IRandomGenerator rndGen;
		private readonly ILineGeneratorSettings settings;
		private readonly IPartGenerator numberPartGenerator;
		private readonly IPartGenerator stringPartGenerator;

		private readonly byte[] result;
		private readonly byte[] randomBytes;

		public LineGenerator(
			IRandomGenerator rndGen,
			ILineGeneratorSettings settings,
			IPartGenerator numberPartGenerator,
			IPartGenerator stringPartGenerator)
		{
			this.rndGen = rndGen;
			this.settings = settings;
			this.numberPartGenerator = numberPartGenerator;
			this.stringPartGenerator = stringPartGenerator;

			result = new byte[settings.MaxLineLength];
			randomBytes = new byte[settings.MaxLineLength];
		}

		public LineInfo GetNewLine()
		{
			var stringLength = rndGen.Next(settings.MinLineLength, settings.MaxLineLength + 1);

			rndGen.NextBytes(randomBytes, 0, stringLength);

			var numberPartLength = rndGen.Next(settings.MinNumberLength, settings.MaxNumberLength + 1);
			var stringPartStartIndex = numberPartLength + settings.PartsSeparatorBytes.Length;
			var stringPartLength = stringLength - stringPartStartIndex;

			numberPartGenerator.Fill(
				result,
				0,
				randomBytes.AsSpan(0, numberPartLength));

			settings.PartsSeparatorBytes.CopyTo(result, numberPartLength);

			stringPartGenerator.Fill(
				result,
				stringPartStartIndex,
				randomBytes.AsSpan(
					stringPartStartIndex,
					stringPartLength));

			return
				new LineInfo(
					result.AsSpan(0, stringLength),
					result.AsSpan(0, numberPartLength),
					result.AsSpan(stringPartStartIndex, stringPartLength));
		}
	}
}
