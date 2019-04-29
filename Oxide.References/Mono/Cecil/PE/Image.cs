using Mono;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Metadata;
using System;

namespace Mono.Cecil.PE
{
	internal sealed class Image
	{
		public ModuleKind Kind;

		public string RuntimeVersion;

		public TargetArchitecture Architecture;

		public ModuleCharacteristics Characteristics;

		public string FileName;

		public Section[] Sections;

		public Section MetadataSection;

		public uint EntryPointToken;

		public ModuleAttributes Attributes;

		public DataDirectory Debug;

		public DataDirectory Resources;

		public DataDirectory StrongName;

		public Mono.Cecil.Metadata.StringHeap StringHeap;

		public Mono.Cecil.Metadata.BlobHeap BlobHeap;

		public Mono.Cecil.Metadata.UserStringHeap UserStringHeap;

		public Mono.Cecil.Metadata.GuidHeap GuidHeap;

		public Mono.Cecil.Metadata.TableHeap TableHeap;

		private readonly int[] coded_index_sizes = new int[13];

		private readonly Func<Table, int> counter;

		public Image()
		{
			this.counter = new Func<Table, int>(this.GetTableLength);
		}

		public int GetCodedIndexSize(CodedIndex coded_index)
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

		public ImageDebugDirectory GetDebugHeader(out byte[] header)
		{
			Section sectionAtVirtualAddress = this.GetSectionAtVirtualAddress(this.Debug.VirtualAddress);
			ByteBuffer byteBuffer = new ByteBuffer(sectionAtVirtualAddress.Data)
			{
				position = (int)(this.Debug.VirtualAddress - sectionAtVirtualAddress.VirtualAddress)
			};
			ImageDebugDirectory imageDebugDirectory = new ImageDebugDirectory()
			{
				Characteristics = byteBuffer.ReadInt32(),
				TimeDateStamp = byteBuffer.ReadInt32(),
				MajorVersion = byteBuffer.ReadInt16(),
				MinorVersion = byteBuffer.ReadInt16(),
				Type = byteBuffer.ReadInt32(),
				SizeOfData = byteBuffer.ReadInt32(),
				AddressOfRawData = byteBuffer.ReadInt32(),
				PointerToRawData = byteBuffer.ReadInt32()
			};
			ImageDebugDirectory imageDebugDirectory1 = imageDebugDirectory;
			if (imageDebugDirectory1.SizeOfData == 0 || imageDebugDirectory1.PointerToRawData == 0)
			{
				header = Empty<byte>.Array;
				return imageDebugDirectory1;
			}
			byteBuffer.position = (int)((long)imageDebugDirectory1.PointerToRawData - (ulong)sectionAtVirtualAddress.PointerToRawData);
			header = new byte[imageDebugDirectory1.SizeOfData];
			Buffer.BlockCopy(byteBuffer.buffer, byteBuffer.position, header, 0, (int)header.Length);
			return imageDebugDirectory1;
		}

		public Section GetSection(string name)
		{
			Section[] sections = this.Sections;
			for (int i = 0; i < (int)sections.Length; i++)
			{
				Section section = sections[i];
				if (section.Name == name)
				{
					return section;
				}
			}
			return null;
		}

		public Section GetSectionAtVirtualAddress(uint rva)
		{
			Section[] sections = this.Sections;
			for (int i = 0; i < (int)sections.Length; i++)
			{
				Section section = sections[i];
				if (rva >= section.VirtualAddress && rva < section.VirtualAddress + section.SizeOfRawData)
				{
					return section;
				}
			}
			return null;
		}

		public int GetTableIndexSize(Table table)
		{
			if (this.GetTableLength(table) >= 65536)
			{
				return 4;
			}
			return 2;
		}

		public int GetTableLength(Table table)
		{
			return (int)this.TableHeap[table].Length;
		}

		public bool HasTable(Table table)
		{
			return this.GetTableLength(table) > 0;
		}

		public uint ResolveVirtualAddress(uint rva)
		{
			Section sectionAtVirtualAddress = this.GetSectionAtVirtualAddress(rva);
			if (sectionAtVirtualAddress == null)
			{
				throw new ArgumentOutOfRangeException();
			}
			return this.ResolveVirtualAddressInSection(rva, sectionAtVirtualAddress);
		}

		public uint ResolveVirtualAddressInSection(uint rva, Section section)
		{
			return rva + section.PointerToRawData - section.VirtualAddress;
		}
	}
}