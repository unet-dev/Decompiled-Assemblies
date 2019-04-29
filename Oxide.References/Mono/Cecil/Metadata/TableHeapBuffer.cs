using Mono.Cecil;
using Mono.Cecil.PE;
using System;

namespace Mono.Cecil.Metadata
{
	internal sealed class TableHeapBuffer : HeapBuffer
	{
		private readonly ModuleDefinition module;

		private readonly MetadataBuilder metadata;

		internal MetadataTable[] tables = new MetadataTable[45];

		private bool large_string;

		private bool large_blob;

		private readonly int[] coded_index_sizes = new int[13];

		private readonly Func<Table, int> counter;

		public override bool IsEmpty
		{
			get
			{
				return false;
			}
		}

		public TableHeapBuffer(ModuleDefinition module, MetadataBuilder metadata) : base(24)
		{
			this.module = module;
			this.metadata = metadata;
			this.counter = new Func<Table, int>(this.GetTableLength);
		}

		public void FixupData(uint data_rva)
		{
			FieldRVATable table = this.GetTable<FieldRVATable>(Table.FieldRVA);
			if (table.length == 0)
			{
				return;
			}
			int num = (this.GetTable<FieldTable>(Table.Field).IsLarge ? 4 : 2);
			int num1 = this.position;
			this.position = table.position;
			for (int i = 0; i < table.length; i++)
			{
				uint num2 = base.ReadUInt32();
				this.position -= 4;
				base.WriteUInt32(num2 + data_rva);
				this.position += num;
			}
			this.position = num1;
		}

		private int GetCodedIndexSize(CodedIndex coded_index)
		{
			int codedIndex = (int)coded_index;
			int codedIndexSizes = this.coded_index_sizes[codedIndex];
			if (codedIndexSizes != 0)
			{
				return codedIndexSizes;
			}
			int[] numArray = this.coded_index_sizes;
			int size = coded_index.GetSize(this.counter);
			int num = size;
			numArray[codedIndex] = size;
			return num;
		}

		private byte GetHeapSizes()
		{
			byte num = 0;
			if (this.metadata.string_heap.IsLarge)
			{
				this.large_string = true;
				num = (byte)(num | 1);
			}
			if (this.metadata.blob_heap.IsLarge)
			{
				this.large_blob = true;
				num = (byte)(num | 4);
			}
			return num;
		}

		public TTable GetTable<TTable>(Table table)
		where TTable : MetadataTable, new()
		{
			TTable tTable = (TTable)this.tables[(int)table];
			if (tTable != null)
			{
				return tTable;
			}
			tTable = Activator.CreateInstance<TTable>();
			this.tables[(int)table] = tTable;
			return tTable;
		}

		private byte GetTableHeapVersion()
		{
			TargetRuntime runtime = this.module.Runtime;
			if (runtime != TargetRuntime.Net_1_0 && runtime != TargetRuntime.Net_1_1)
			{
				return (byte)2;
			}
			return (byte)1;
		}

		private int GetTableLength(Table table)
		{
			MetadataTable metadataTable = this.tables[(int)table];
			if (metadataTable == null)
			{
				return 0;
			}
			return metadataTable.Length;
		}

		private ulong GetValid()
		{
			ulong num = (ulong)0;
			for (int i = 0; i < (int)this.tables.Length; i++)
			{
				MetadataTable metadataTable = this.tables[i];
				if (metadataTable != null && metadataTable.Length != 0)
				{
					metadataTable.Sort();
					num = num | (long)1 << (i & 63);
				}
			}
			return num;
		}

		public void WriteBlob(uint blob)
		{
			this.WriteBySize(blob, this.large_blob);
		}

		public void WriteBySize(uint value, int size)
		{
			if (size == 4)
			{
				base.WriteUInt32(value);
				return;
			}
			base.WriteUInt16((ushort)value);
		}

		public void WriteBySize(uint value, bool large)
		{
			if (large)
			{
				base.WriteUInt32(value);
				return;
			}
			base.WriteUInt16((ushort)value);
		}

		public void WriteCodedRID(uint rid, CodedIndex coded_index)
		{
			this.WriteBySize(rid, this.GetCodedIndexSize(coded_index));
		}

		public void WriteRID(uint rid, Table table)
		{
			MetadataTable metadataTable = this.tables[(int)table];
			this.WriteBySize(rid, (metadataTable == null ? false : metadataTable.IsLarge));
		}

		private void WriteRowCount()
		{
			for (int i = 0; i < (int)this.tables.Length; i++)
			{
				MetadataTable metadataTable = this.tables[i];
				if (metadataTable != null && metadataTable.Length != 0)
				{
					base.WriteUInt32((uint)metadataTable.Length);
				}
			}
		}

		public void WriteString(uint @string)
		{
			this.WriteBySize(@string, this.large_string);
		}

		public void WriteTableHeap()
		{
			base.WriteUInt32(0);
			base.WriteByte(this.GetTableHeapVersion());
			base.WriteByte(0);
			base.WriteByte(this.GetHeapSizes());
			base.WriteByte(10);
			base.WriteUInt64(this.GetValid());
			base.WriteUInt64(24190111578624L);
			this.WriteRowCount();
			this.WriteTables();
		}

		private void WriteTables()
		{
			for (int i = 0; i < (int)this.tables.Length; i++)
			{
				MetadataTable metadataTable = this.tables[i];
				if (metadataTable != null && metadataTable.Length != 0)
				{
					metadataTable.Write(this);
				}
			}
		}
	}
}