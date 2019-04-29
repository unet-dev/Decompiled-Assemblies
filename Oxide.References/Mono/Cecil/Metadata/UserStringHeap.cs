using Mono.Cecil;
using Mono.Cecil.PE;
using System;

namespace Mono.Cecil.Metadata
{
	internal sealed class UserStringHeap : StringHeap
	{
		public UserStringHeap(Mono.Cecil.PE.Section section, uint start, uint size) : base(section, start, size)
		{
		}

		protected override string ReadStringAt(uint index)
		{
			byte[] data = this.Section.Data;
			int num = (int)(index + this.Offset);
			uint num1 = (uint)((ulong)data.ReadCompressedUInt32(ref num) & (long)-2);
			if (num1 < 1)
			{
				return string.Empty;
			}
			char[] chrArray = new char[num1 / 2];
			int num2 = num;
			int num3 = 0;
			while ((long)num2 < (long)num + (ulong)num1)
			{
				int num4 = num3;
				num3 = num4 + 1;
				chrArray[num4] = (char)(data[num2] | data[num2 + 1] << 8);
				num2 += 2;
			}
			return new string(chrArray);
		}
	}
}