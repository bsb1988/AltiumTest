using FileSorter.Generator.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;

namespace FileSorter.Generator
{
	public class TestFileGenerator
	{
		private readonly IFileSettings fileSettings;
		private readonly ILineGenerator lineWriter;
		private readonly Stream stream;

		public TestFileGenerator(
			IFileSettings fileSettings,
			ILineGenerator lineWriter,
			Stream stream)
		{
			this.fileSettings = fileSettings;
			this.lineWriter = lineWriter;
			this.stream = stream;
		}

		public void Generate()
		{
			ulong fullSize = 0;
			LineInfo line;

			while (fullSize < fileSettings.FileSize)
			{
				if (fullSize > 0)
				{
					stream.Write(fileSettings.LineSeparatorBytes);
					fullSize++;
				}
				line = lineWriter.GetNewLine();
				stream.Write(line.Line);
				fullSize += (ulong)line.Line.Length;
			}
		}
	}
}
