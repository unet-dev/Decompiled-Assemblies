using Mono.Cecil.PE;
using System;
using System.Reflection;

namespace Mono.Cecil.Metadata
{
	internal sealed class TableHeap : Heap
	{
		public long Valid;

		public long Sorted;

		public const int TableCount = 45;

		public readonly TableInformation[] Tables = new TableInformation[45];

		public TableInformation this[Table table]
		{
			get
			{
				return this.Tables[(int)table];
			}
		}

		public TableHeap(Mono.Cecil.PE.Section section, uint start, uint size) : base(section, start, size)
		{
		}

		public bool HasTable(Table table)
		{
			return (this.Valid & (byte)Table.TypeRef << (byte)(table & (Table.TypeRef | Table.TypeDef | Table.FieldPtr | Table.Field | Table.MethodPtr | Table.Method | Table.ParamPtr | Table.Param | Table.InterfaceImpl | Table.MemberRef | Table.Constant | Table.CustomAttribute | Table.FieldMarshal | Table.DeclSecurity | Table.ClassLayout | Table.FieldLayout | Table.StandAloneSig | Table.EventMap | Table.EventPtr | Table.Event | Table.PropertyMap | Table.PropertyPtr | Table.Property | Table.MethodSemantics | Table.MethodImpl | Table.ModuleRef | Table.TypeSpec | Table.ImplMap | Table.FieldRVA | Table.EncLog | Table.EncMap | Table.Assembly | Table.AssemblyProcessor | Table.AssemblyOS | Table.AssemblyRef | Table.AssemblyRefProcessor | Table.AssemblyRefOS | Table.File | Table.ExportedType | Table.ManifestResource | Table.NestedClass | Table.GenericParam | Table.MethodSpec | Table.GenericParamConstraint))) != (Int64)Table.Module;
		}
	}
}