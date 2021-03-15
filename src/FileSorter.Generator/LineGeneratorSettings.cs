using FileSorter.Generator.Interfaces;
using System;
using System.Text;

namespace FileSorter.Generator
{
	public class LineGeneratorSettings : ILineGeneratorSettings
	{
		public byte[] PartsSeparatorBytes { get; }

		public ushort MinNumberLength { get; }
		public ushort MaxNumberLength { get; }
		public ushort MinLineLength { get; }
		public ushort MaxLineLength { get; }

		public LineGeneratorSettings(
			ushort minNumberLength,
			ushort maxNumberLength,
			ushort minLineLength,
			ushort maxLineLength,
			string partsSeparator)
		{
			PartsSeparatorBytes = Encoding.ASCII.GetBytes(partsSeparator);

			if (minNumberLength == 0)
			{
				throw new ArgumentException("Min number part length should be greater than 0.");
			}

			if (maxNumberLength < minNumberLength)
			{
				throw new ArgumentException("Max number part length should be greater than or equal to min number part length.");
			}

			if (minLineLength == 0)
			{
				throw new ArgumentException("Min line length should be greater than 0.");
			}

			if (minLineLength <= maxNumberLength + PartsSeparatorBytes.Length)
			{
				throw new ArgumentException("Min line lenght should be greater than max number part length at least on parts separator length.");
			}

			if (maxLineLength < minLineLength)
			{
				throw new ArgumentException("Max line length should be greater than min line length.");
			}

			if (maxLineLength > 1024)
			{
				throw new ArgumentException("Max line length should not be greater than 1024.");
			}

			MinNumberLength = minNumberLength;
			MaxNumberLength = maxNumberLength;
			MinLineLength = minLineLength;
			MaxLineLength = maxLineLength;
		}
	}
}
