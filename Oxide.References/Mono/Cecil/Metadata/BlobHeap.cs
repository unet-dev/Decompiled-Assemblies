using Mono;
using Mono.Cecil;
using Mono.Cecil.PE;
using System;

namespace Mono.Cecil.Metadata
{
	internal sealed class BlobHeap : Heap
	{
		public BlobHeap(Mono.Cecil.PE.Section section, uint start, uint size) : base(section, start, size)
		{
		}

		public byte[] Read(uint index)
		{
			if (index == 0 || index > this.Size - 1)
			{
				return Empty<byte>.Array;
			}
			byte[] data = this.Section.Data;
			int num = (int)(index + this.Offset);
			int num1 = (int)data.ReadCompressedUInt32(ref num);
			byte[] numArray = new byte[num1];
			Buffer.BlockCopy((Array)data, num, numArray, 0, num1);
			return numArray;
		}
	}
}