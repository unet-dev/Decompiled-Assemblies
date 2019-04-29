using Mono.Cecil.Metadata;
using Mono.Cecil.PE;
using System;

namespace Mono.Cecil
{
	internal sealed class ClassLayoutTable : SortedTable<Row<ushort, uint, uint>>
	{
		public ClassLayoutTable()
		{
		}

		public override int Compare(Row<ushort, uint, uint> x, Row<ushort, uint, uint> y)
		{
			return base.Compare((uint)x.Col3, (uint)y.Col3);
		}

		public override void Write(TableHeapBuffer buffer)
		{
			for (int i = 0; i < this.length; i++)
			{
				buffer.WriteUInt16(this.rows[i].Col1);
				buffer.WriteUInt32(this.rows[i].Col2);
				buffer.WriteRID(this.rows[i].Col3, Table.TypeDef);
			}
		}
	}
}