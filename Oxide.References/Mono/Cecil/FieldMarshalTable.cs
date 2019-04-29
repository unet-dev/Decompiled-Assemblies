using Mono.Cecil.Metadata;
using System;

namespace Mono.Cecil
{
	internal sealed class FieldMarshalTable : SortedTable<Row<uint, uint>>
	{
		public FieldMarshalTable()
		{
		}

		public override int Compare(Row<uint, uint> x, Row<uint, uint> y)
		{
			return base.Compare((uint)x.Col1, (uint)y.Col1);
		}

		public override void Write(TableHeapBuffer buffer)
		{
			for (int i = 0; i < this.length; i++)
			{
				buffer.WriteCodedRID(this.rows[i].Col1, CodedIndex.HasFieldMarshal);
				buffer.WriteBlob(this.rows[i].Col2);
			}
		}
	}
}