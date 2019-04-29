using System;

namespace Mono.Cecil
{
	internal abstract class OneRowTable<TRow> : MetadataTable
	where TRow : struct
	{
		internal TRow row;

		public sealed override int Length
		{
			get
			{
				return 1;
			}
		}

		protected OneRowTable()
		{
		}

		public sealed override void Sort()
		{
		}
	}
}