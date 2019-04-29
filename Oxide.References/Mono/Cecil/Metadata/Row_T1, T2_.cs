using System;

namespace Mono.Cecil.Metadata
{
	internal struct Row<T1, T2>
	{
		internal T1 Col1;

		internal T2 Col2;

		public Row(T1 col1, T2 col2)
		{
			this.Col1 = col1;
			this.Col2 = col2;
		}
	}
}