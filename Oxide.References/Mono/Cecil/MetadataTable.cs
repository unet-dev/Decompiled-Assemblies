using Mono.Cecil.Metadata;
using System;

namespace Mono.Cecil
{
	internal abstract class MetadataTable
	{
		public bool IsLarge
		{
			get
			{
				return this.Length > 65535;
			}
		}

		public abstract int Length
		{
			get;
		}

		protected MetadataTable()
		{
		}

		public abstract void Sort();

		public abstract void Write(TableHeapBuffer buffer);
	}
}