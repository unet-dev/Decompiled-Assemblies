using Mono.Cecil.Metadata;
using System;

namespace Mono.Cecil
{
	internal sealed class MethodSpecTable : MetadataTable<Row<uint, uint>>
	{
		public MethodSpecTable()
		{
		}

		public override void Write(TableHeapBuffer buffer)
		{
			for (int i = 0; i < this.length; i++)
			{
				buffer.WriteCodedRID(this.rows[i].Col1, CodedIndex.MethodDefOrRef);
				buffer.WriteBlob(this.rows[i].Col2);
			}
		}
	}
}