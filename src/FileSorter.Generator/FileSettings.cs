using FileSorter.Generator.Interfaces;
using System.Text;

namespace FileSorter.Generator
{
	public class FileSettings : IFileSettings
	{
		private readonly ulong fileSize;
		private readonly byte[] lineSeparatorBytes;

		public ulong FileSize => fileSize;
		public byte[] LineSeparatorBytes => lineSeparatorBytes;

		public FileSettings(ulong fileSize, string lineSeparator)
		{
			this.fileSize = fileSize;
			this.lineSeparatorBytes = Encoding.ASCII.GetBytes(lineSeparator);
		}
	}
}
