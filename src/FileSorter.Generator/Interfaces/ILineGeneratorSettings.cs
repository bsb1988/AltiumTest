namespace FileSorter.Generator.Interfaces
{
	public interface ILineGeneratorSettings
	{
		ushort MaxLineLength { get; }
		ushort MaxNumberLength { get; }
		ushort MinLineLength { get; }
		ushort MinNumberLength { get; }
		byte[] PartsSeparatorBytes { get; }
	}
}