using Mono.Cecil.Metadata;
using System;

namespace Mono.Cecil
{
	internal sealed class TypeRefTable : MetadataTable<Row<uint, uint, uint>>
	{
		public TypeRefTable()
		{
		}

		public override void Write(TableHeapBuffer buffer)
		{
			for (int i = 0; i < this.length; i++)
			{
				buffer.WriteCodedRID(this.rows[i].Col1, CodedIndex.ResolutionScope);
				buffer.WriteString(this.rows[i].Col2);
				buffer.WriteString(this.rows[i].Col3);
			}
		}
	}
}