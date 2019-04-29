using Mono.Cecil.PE;
using System;

namespace Mono.Cecil.Metadata
{
	internal sealed class GuidHeap : Heap
	{
		public GuidHeap(Mono.Cecil.PE.Section section, uint start, uint size) : base(section, start, size)
		{
		}

		public Guid Read(uint index)
		{
			if (index == 0)
			{
				return new Guid();
			}
			byte[] numArray = new byte[16];
			index--;
			Buffer.BlockCopy(this.Section.Data, (int)(this.Offset + index), numArray, 0, 16);
			return new Guid(numArray);
		}
	}
}