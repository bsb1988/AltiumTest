using NUnit.Framework;
using System.Text;

namespace FileSorter.Generator.Tests
{
	[TestFixture]
	public class LineGeneratorSettingsTests
	{
		[TestCase(1, 1, 3, 3, ".", "\n")]
		[TestCase(1, 2, 5, 6, ".", "")]
		[TestCase(1, 1, 6, 7, "....", "\n\r")]
		[TestCase(1, 1, 3, 3, "", "\n")]
		[TestCase(1, 10, 30, 1024, ". ", "\n")]
		public void ShouldBeCreatedWithValidParameters(
			int minNumberLength = 1,
			int maxNumberLength = 1,
			int minLineLength = 4,
			int maxLineLength = 4,
			string partsSeparator = ". ",
			string lineSeparator = "\n")
		{
			var settings = CreateSettings(
				minNumberLength: (ushort)minNumberLength,
				maxNumberLength: (ushort)maxNumberLength,
				minLineLength: (ushort)minLineLength,
				maxLineLength: (ushort)maxLineLength,
				partsSeparator: partsSeparator);

			Assert.That(settings.MinNumberLength, Is.EqualTo(minNumberLength));
			Assert.That(settings.MaxNumberLength, Is.EqualTo(maxNumberLength));
			Assert.That(settings.MinLineLength, Is.EqualTo(minLineLength));
			Assert.That(settings.MaxLineLength, Is.EqualTo(maxLineLength));
			Assert.That(Encoding.ASCII.GetString(settings.PartsSeparatorBytes), Is.EqualTo(partsSeparator));
		}

		[Test]
		public void NumberPart_MinLengthShouldBeGreaterThanZeroTest()
		{
			Assert.That(
				() => CreateSettings(minNumberLength: 0),
				Throws.ArgumentException
				.And
				.Message
				.EqualTo("Min number part length should be greater than 0."));
		}

		[Test]
		public void NumberPart_MinLengthShouldNotBeGreaterThanMaxLengthTest()
		{
			Assert.That(
				() => CreateSettings(
					minNumberLength: 1,
					maxNumberLength: 0),
				Throws.ArgumentException
				.And.Message
				.EqualTo("Max number part length should be greater than or equal to min number part length."));
		}

		[Test]
		public void Line_MinLengthShouldBeGreaterThanZeroTest()
		{
			Assert.That(
				() => CreateSettings(minLineLength: 0),
				Throws.ArgumentException
				.And
				.Message
				.EqualTo("Min line length should be greater than 0."));
		}

		[TestCase(10, 5, "")]
		[TestCase(10, 10, ".")]
		[TestCase(10, 11, ".")]
		[TestCase(10, 12, "..")]
		[TestCase(10, 12, ".....")]
		public void Line_MaxNumberPartLengthPlusPartsSeparatorLengthShouldBeLessThanMinLineLenth(
			int maxNumberLength,
			int minLineLength,
			string partsSeparator)
		{
			Assert.That(
				() => CreateSettings(
					maxNumberLength: (ushort)maxNumberLength,
					minLineLength: (ushort)minLineLength,
					partsSeparator: partsSeparator),
				Throws.ArgumentException
				.And.Message
				.EqualTo("Min line lenght should be greater than max number part length at least on parts separator length."));
		}

		[Test]
		public void Line_MinLengthShouldNotBeGreaterThanMaxLengthTest()
		{
			Assert.That(
				() => CreateSettings(
					minLineLength: 10,
					maxLineLength: 9),
				Throws.ArgumentException
				.And.Message
				.EqualTo("Max line length should be greater than min line length."));
		}

		[Test]
		public void Line_MaxLengthShouldNotBeGreaterThan1024Test()
		{
			Assert.That(
				() => CreateSettings(maxLineLength: 1025),
				Throws.ArgumentException
				.And
				.Message
				.EqualTo("Max line length should not be greater than 1024."));
		}


		private LineGeneratorSettings CreateSettings(
			ushort minNumberLength = 1,
			ushort maxNumberLength = 1,
			ushort minLineLength = 4,
			ushort maxLineLength = 4,
			string partsSeparator = ". ") =>
			new LineGeneratorSettings(
				minNumberLength,
				maxNumberLength,
				minLineLength,
				maxLineLength,
				partsSeparator);
	}
}
