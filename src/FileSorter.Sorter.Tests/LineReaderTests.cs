using System;
using NUnit.Framework;
using System.Linq;
using System.Diagnostics;
using System.IO;
using NSubstitute;
using System.Text;
using System.Collections.Generic;

namespace FileSorter.Sorter.Tests
{
	[TestFixture]
	public class LineReaderTests
	{
		[Test]
		public void SingleLinePartsSeparatorIndexTest()
		{
			var originalLine = new Line(Encoding.ASCII.GetBytes("1. S1"), 3);

			var stream = new MemoryStream(originalLine.LineBytes.ToArray());

			var reader = new LineReader(stream, originalLine.LineBytes.Length);
			var lines = reader.ToList();

			Assert.That(lines.Count, Is.EqualTo(1));
			Assert.That(lines[0].CompareTo(originalLine), Is.EqualTo(0));
		}

		private void CompareLines(IEnumerable<Line> expectedLines, IEnumerable<Line> currentLines)
		{
			Assert.That(
				currentLines.Select(l => Encoding.ASCII.GetString(l.LineBytes)),
				Is.EquivalentTo(expectedLines.Select(l => Encoding.ASCII.GetString(l.LineBytes))));
			foreach (var line in expectedLines)
			{
				var currentLine = currentLines.First(cl => cl.LineBytes.SequenceEqual(line.LineBytes));
				Assert.That(currentLine.CompareTo(line), Is.EqualTo(0));
			}
		}

		[Test]
		public void SimpleScenarioNoNewLineAtTheEndTest()
		{
			var originalLines = new List<Line>
			{
				new Line(Encoding.ASCII.GetBytes("1. S1"), 3),
				new Line(Encoding.ASCII.GetBytes("2. S2"), 3),
			};
			var testStringBytes = Encoding.ASCII.GetBytes(string.Join('\n', originalLines.Select(l => Encoding.ASCII.GetString(l.LineBytes))));

			var stream = new MemoryStream(testStringBytes);

			var reader = new LineReader(stream, testStringBytes.Length);
			var lines = reader.ToList();

			CompareLines(originalLines, lines);
		}

		[Test]
		public void LotsOfLinesTest()
		{
			var originalLines =
				Enumerable.Range(0, 20)
				.Select(i => new Line(Encoding.ASCII.GetBytes($"1. S{i}"), 3))
				.ToList();

			var testStringBytes = Encoding.ASCII.GetBytes(
				string.Join('\n', originalLines.Select(l => Encoding.ASCII.GetString(l.LineBytes))));

			var stream = new MemoryStream(testStringBytes);

			var reader = new LineReader(stream, testStringBytes.Length);
			var lines = reader.ToList();

			CompareLines(originalLines, lines);
		}

		[Test]
		public void SimpleScenarioNewLineAtTheEndTest()
		{
			var originalLines = new List<string>
			{
				"1. S1", "2. S2"
			};
			var testStringBytes = Encoding.ASCII.GetBytes(string.Join('\n', originalLines) + '\n');

			var stream = new MemoryStream(testStringBytes);

			var reader = new LineReader(stream, testStringBytes.Length);
			var lines = reader.ToList();

			Assert.That(lines.Select(l => Encoding.ASCII.GetString(l.LineBytes)), Is.EquivalentTo(originalLines));
		}

		[Test]
		public void TwoBatchesScenarioNoNewLineAtTheEndTest()
		{
			var originalLines = new List<string>
			{
				"1. S1", "2. S2"
			};
			var testStringBytes = Encoding.ASCII.GetBytes(string.Join('\n', originalLines));

			var stream = new MemoryStream(testStringBytes);

			var reader = new LineReader(stream, 7);
			var lines = reader.ToList();

			Assert.That(lines.Select(l => Encoding.ASCII.GetString(l.LineBytes)), Is.EquivalentTo(originalLines));
		}

		[Test]
		public void TwoBatchesScenarioNewLineAtTheEndTest()
		{
			var originalLines = new List<string>
			{
				"1. S1", "2. S2"
			};
			var testStringBytes = Encoding.ASCII.GetBytes(string.Join('\n', originalLines) + '\n');

			var stream = new MemoryStream(testStringBytes);

			var reader = new LineReader(stream, 7);
			var lines = reader.ToList();

			Assert.That(lines.Select(l => Encoding.ASCII.GetString(l.LineBytes)), Is.EquivalentTo(originalLines));
		}

		[Test]
		public void SpecifyStartIndexAndLengthTest()
		{
			var originalLines = new List<string>
			{
				"1. S1", "2. S2", "3. S3", "4. S4", "5. S5", "6. S6", "7. S7"
			};
			var testStringBytes = Encoding.ASCII.GetBytes(string.Join('\n', originalLines) + '\n');

			var expectedLines = new List<string> { originalLines[2], originalLines[3] };

			var stream = new MemoryStream(testStringBytes);

			var reader = new LineReader(stream, 100, 12, 11);
			var lines = reader.ToList();

			Assert.That(lines.Select(l => Encoding.ASCII.GetString(l.LineBytes)), Is.EquivalentTo(expectedLines));
		}
	}
}
