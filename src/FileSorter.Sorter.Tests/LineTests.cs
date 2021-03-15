using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using System.Text;

namespace FileSorter.Sorter.Tests
{
	[TestFixture]
	public class LineTests
	{
		[Test]
		public void DifferentStringPartTest()
		{
			var str1 = "1. S1";
			var str2 = "2. S2";

			var line1 = new Line(Encoding.ASCII.GetBytes(str1), 1);
			var line2 = new Line(Encoding.ASCII.GetBytes(str2), 1);

			Assert.That(line1.CompareTo(line2), Is.LessThan(0));
		}

		[Test]
		public void SameStringPartSameNumberPartLengthTest()
		{
			var str1 = "1. S1";
			var str2 = "2. S1";

			var line1 = new Line(Encoding.ASCII.GetBytes(str1), 1);
			var line2 = new Line(Encoding.ASCII.GetBytes(str2), 1);

			Assert.That(line1.CompareTo(line2), Is.LessThan(0));
		}

		[Test]
		public void SameStringPartDifferentNumberPartLengthTest()
		{
			var str1 = "11. S1";
			var str2 = "2. S1";

			var line1 = new Line(Encoding.ASCII.GetBytes(str1), 2);
			var line2 = new Line(Encoding.ASCII.GetBytes(str2), 1);

			Assert.That(line1.CompareTo(line2), Is.GreaterThan(0));
		}

		[Test]
		public void EqualLinesTest()
		{
			var str = "11. S1";

			var line1 = new Line(Encoding.ASCII.GetBytes(str), 2);
			var line2 = new Line(Encoding.ASCII.GetBytes(str), 2);

			Assert.That(line1.CompareTo(line2), Is.EqualTo(0));
		}
	}
}
