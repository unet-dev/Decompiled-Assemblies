using Mono.Cecil.PE;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mono.Cecil.Metadata
{
	internal class StringHeap : Heap
	{
		private readonly Dictionary<uint, string> strings = new Dictionary<uint, string>();

		public StringHeap(Mono.Cecil.PE.Section section, uint start, uint size) : base(section, start, size)
		{
		}

		public string Read(uint index)
		{
			string str;
			if (index == 0)
			{
				return string.Empty;
			}
			if (this.strings.TryGetValue(index, out str))
			{
				return str;
			}
			if (index > this.Size - 1)
			{
				return string.Empty;
			}
			str = this.ReadStringAt(index);
			if (str.Length != 0)
			{
				this.strings.Add(index, str);
			}
			return str;
		}

		protected virtual string ReadStringAt(uint index)
		{
			int num = 0;
			byte[] data = this.Section.Data;
			int num1 = (int)(index + this.Offset);
			for (int i = num1; data[i] != 0; i++)
			{
				num++;
			}
			return Encoding.UTF8.GetString(data, num1, num);
		}
	}
}