using Mono.Cecil.Metadata;
using System;

namespace Mono.Cecil
{
	internal sealed class InterfaceImplTable : MetadataTable<Row<uint, uint>>
	{
		public InterfaceImplTable()
		{
		}

		public override void Write(TableHeapBuffer buffer)
		{
			for (int i = 0; i < this.length; i++)
			{
				buffer.WriteRID(this.rows[i].Col1, Table.TypeDef);
				buffer.WriteCodedRID(this.rows[i].Col2, CodedIndex.TypeDefOrRef);
			}
		}
	}
}