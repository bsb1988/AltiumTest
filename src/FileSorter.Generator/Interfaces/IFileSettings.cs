namespace FileSorter.Generator.Interfaces
{
	public interface IFileSettings
	{
		ulong FileSize { get; }
		byte[] LineSeparatorBytes { get; }
	}
}
