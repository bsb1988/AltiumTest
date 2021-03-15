using NUnit.Framework;
using System;
using System.IO;
using NSubstitute;
using System.Linq;
using System.Text;
using FileSorter.Generator.Interfaces;

namespace FileSorter.Generator.Tests
{
	[TestFixture]
	public class LineWriterTests
	{
		[Test]
		public void Test()
		{
			var settings = new LineGeneratorSettings(1, 5, 10, 128, ". ");

			const int expectedStringLength = 10;
			const int expectedNumberPartLength = 1;

			var expectedNextBytes = Enumerable.Range(1, expectedStringLength).Select(x => (byte)x).ToArray();

			var expectedNumberPart = "2";
			var expectedStringPart = "456789a";

			var expectedString = $"{expectedNumberPart}{Encoding.ASCII.GetString(settings.PartsSeparatorBytes)}{expectedStringPart}";

			var randomGenerator = Substitute.For<IRandomGenerator>();
			randomGenerator
				.Next(settings.MinLineLength, settings.MaxLineLength + 1)
				.Returns(expectedStringLength);
			randomGenerator
				.Next(settings.MinNumberLength, settings.MaxNumberLength + 1)
				.Returns(expectedNumberPartLength);

			randomGenerator
				.When(x => x.NextBytes(Arg.Any<byte[]>(), Arg.Any<int>(), Arg.Any<int>()))
				.Do(callInfo => expectedNextBytes.CopyTo(callInfo.ArgAt<byte[]>(0), 0));

			var stream = Substitute.For<Stream>();

			var lineWriter = new LineGenerator(
				randomGenerator,
				settings,
				new NumberPartGenerator(),
				new StringPartGenerator());
			var lineInfo = lineWriter.GetNewLine();

			Assert.That(
				Encoding.ASCII.GetString(lineInfo.Line),
				Is.EqualTo(expectedString));
			Assert.That(
				Encoding.ASCII.GetString(lineInfo.NumberPart),
				Is.EqualTo(expectedNumberPart));
			Assert.That(
				Encoding.ASCII.GetString(lineInfo.StringPart),
				Is.EqualTo(expectedStringPart));
		}
	}
}
