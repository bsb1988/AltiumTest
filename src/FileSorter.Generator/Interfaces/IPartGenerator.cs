using System;

namespace FileSorter.Generator.Interfaces
{
	public interface IPartGenerator
	{
		void Fill(byte[] targetArray, int startIndex, Span<byte> randomBytes);
	}
}
