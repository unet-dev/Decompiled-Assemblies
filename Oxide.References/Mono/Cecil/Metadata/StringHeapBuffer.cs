using Mono.Cecil.PE;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mono.Cecil.Metadata
{
	internal class StringHeapBuffer : HeapBuffer
	{
		private readonly Dictionary<string, uint> strings = new Dictionary<string, uint>(StringComparer.Ordinal);

		public sealed override bool IsEmpty
		{
			get
			{
				return this.length <= 1;
			}
		}

		public StringHeapBuffer() : base(1)
		{
			base.WriteByte(0);
		}

		public uint GetStringIndex(string @string)
		{
			uint num;
			if (this.strings.TryGetValue(@string, out num))
			{
				return num;
			}
			num = (uint)this.position;
			this.WriteString(@string);
			this.strings.Add(@string, num);
			return num;
		}

		protected virtual void WriteString(string @string)
		{
			base.WriteBytes(Encoding.UTF8.GetBytes(@string));
			base.WriteByte(0);
		}
	}
}