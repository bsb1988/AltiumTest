using FileSorter.Generator.Interfaces;
using System;
using System.Linq;
using System.Text;

namespace FileSorter.Generator
{
	public class StringPartGenerator : IPartGenerator
	{
		private readonly byte[] allChars;

		public StringPartGenerator()
		{
			allChars = Encoding.ASCII.GetBytes("0123456789")
				.Concat(Enumerable.Range('a', 'z' - 'a' + 1)
					.Concat(Enumerable.Range('A', 'Z' - 'A' + 1))
					.Concat(new[] { (int)' ' })
					.Select(c => (byte)c))
				.ToArray();
		}

		public void Fill(byte[] targetArray, int startIndex, Span<byte> randomBytes)
		{
			for (int i = 0; i < randomBytes.Length; i++)
			{
				targetArray[i + startIndex] = allChars[randomBytes[i] % allChars.Length];
			}
		}
	}
}
