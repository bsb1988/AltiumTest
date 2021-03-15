using FileSorter.Generator.Interfaces;

namespace FileSorter.Generator
{
	public class FastRandomGenerator : IRandomGenerator
	{
		private readonly FastRandom rnd = new FastRandom();

		public int Next(int minValue, int maxValue)
		{
			return rnd.Next(minValue, maxValue);
		}

		public void NextBytes(byte[] bytes, int startIndex, int length)
		{
			rnd.NextBytes(bytes, startIndex, length);
		}
	}
}
