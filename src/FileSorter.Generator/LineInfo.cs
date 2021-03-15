using System;
using System.Runtime.CompilerServices;

namespace FileSorter.Generator
{
	public readonly ref struct LineInfo
	{
		private readonly ReadOnlySpan<byte> line;
		private readonly ReadOnlySpan<byte> numberPart;
		private readonly ReadOnlySpan<byte> stringPart;

		public ReadOnlySpan<byte> Line => line;
		public ReadOnlySpan<byte> NumberPart => numberPart;
		public ReadOnlySpan<byte> StringPart => stringPart;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public LineInfo(
			ReadOnlySpan<byte> line,
			ReadOnlySpan<byte> numberPart,
			ReadOnlySpan<byte> stringPart) : this()
		{
			this.numberPart = numberPart;
			this.stringPart = stringPart;
			this.line = line;
		}
	}
}
