using Mono.Cecil.PE;
using System;

namespace Mono.Cecil.Metadata
{
	internal abstract class Heap
	{
		public int IndexSize;

		public readonly Mono.Cecil.PE.Section Section;

		public readonly uint Offset;

		public readonly uint Size;

		protected Heap(Mono.Cecil.PE.Section section, uint offset, uint size)
		{
			this.Section = section;
			this.Offset = offset;
			this.Size = size;
		}
	}
}