using FileSorter.Generator.Interfaces;
using System;
using System.Text;

namespace FileSorter.Generator
{
	public class NumberPartGenerator : IPartGenerator
	{
		private readonly byte[] numberChars;

		public NumberPartGenerator()
		{
			this.numberChars = Encoding.ASCII.GetBytes("0123456789");
		}

		public void Fill(
			byte[] targetArray,
			int startIndex,
			Span<byte> randomBytes)
		{
			targetArray[startIndex] = numberChars[1 + randomBytes[0] % 9];
			for (int i = 1; i < randomBytes.Length; i++)
			{
				targetArray[i + startIndex] = numberChars[randomBytes[i] % numberChars.Length];
			}
		}
	}
}
