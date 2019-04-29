using Mono.Cecil.Metadata;
using System;

namespace Mono.Cecil
{
	internal sealed class CustomAttributeTable : SortedTable<Row<uint, uint, uint>>
	{
		public CustomAttributeTable()
		{
		}

		public override int Compare(Row<uint, uint, uint> x, Row<uint, uint, uint> y)
		{
			return base.Compare((uint)x.Col1, (uint)y.Col1);
		}

		public override void Write(TableHeapBuffer buffer)
		{
			for (int i = 0; i < this.length; i++)
			{
				buffer.WriteCodedRID(this.rows[i].Col1, CodedIndex.HasCustomAttribute);
				buffer.WriteCodedRID(this.rows[i].Col2, CodedIndex.CustomAttributeType);
				buffer.WriteBlob(this.rows[i].Col3);
			}
		}
	}
}