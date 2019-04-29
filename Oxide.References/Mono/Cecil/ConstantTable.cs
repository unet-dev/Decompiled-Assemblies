using Mono.Cecil.Metadata;
using Mono.Cecil.PE;
using System;

namespace Mono.Cecil
{
	internal sealed class ConstantTable : SortedTable<Row<ElementType, uint, uint>>
	{
		public ConstantTable()
		{
		}

		public override int Compare(Row<ElementType, uint, uint> x, Row<ElementType, uint, uint> y)
		{
			return base.Compare((uint)x.Col2, (uint)y.Col2);
		}

		public override void Write(TableHeapBuffer buffer)
		{
			for (int i = 0; i < this.length; i++)
			{
				buffer.WriteUInt16(this.rows[i].Col1);
				buffer.WriteCodedRID(this.rows[i].Col2, CodedIndex.HasConstant);
				buffer.WriteBlob(this.rows[i].Col3);
			}
		}
	}
}