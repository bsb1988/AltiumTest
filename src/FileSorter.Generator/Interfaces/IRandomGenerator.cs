namespace FileSorter.Generator.Interfaces
{
	public interface IRandomGenerator
	{
		int Next(int minValue, int maxValue);
		void NextBytes(byte[] bytes, int startIndex, int length);
	}
}
