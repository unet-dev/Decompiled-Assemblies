using Mono.Cecil.PE;
using System;

namespace Mono.Cecil.Metadata
{
	internal abstract class HeapBuffer : ByteBuffer
	{
		public abstract bool IsEmpty
		{
			get;
		}

		public bool IsLarge
		{
			get
			{
				return this.length > 65535;
			}
		}

		protected HeapBuffer(int length) : base(length)
		{
		}
	}
}