using Mono.Cecil.Metadata;
using Mono.Cecil.PE;
using System;

namespace Mono.Cecil
{
	internal sealed class ModuleTable : OneRowTable<uint>
	{
		public ModuleTable()
		{
		}

		public override void Write(TableHeapBuffer buffer)
		{
			buffer.WriteUInt16(0);
			buffer.WriteString(this.row);
			buffer.WriteUInt16(1);
			buffer.WriteUInt16(0);
			buffer.WriteUInt16(0);
		}
	}
}