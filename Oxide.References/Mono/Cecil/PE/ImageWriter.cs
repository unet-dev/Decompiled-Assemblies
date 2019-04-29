using Mono;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Metadata;
using System;
using System.IO;

namespace Mono.Cecil.PE
{
	internal sealed class ImageWriter : BinaryStreamWriter
	{
		private readonly ModuleDefinition module;

		private readonly MetadataBuilder metadata;

		private readonly TextMap text_map;

		private ImageDebugDirectory debug_directory;

		private byte[] debug_data;

		private ByteBuffer win32_resources;

		private const uint pe_header_size = 152;

		private const uint section_header_size = 40;

		private const uint file_alignment = 512;

		private const uint section_alignment = 8192;

		private const ulong image_base = 4194304L;

		internal const uint text_rva = 8192;

		private readonly bool pe64;

		private readonly bool has_reloc;

		private readonly uint time_stamp;

		internal Section text;

		internal Section rsrc;

		internal Section reloc;

		private ushort sections;

		private ImageWriter(ModuleDefinition module, MetadataBuilder metadata, Stream stream) : base(stream)
		{
			this.module = module;
			this.metadata = metadata;
			this.pe64 = (module.Architecture == TargetArchitecture.AMD64 ? true : module.Architecture == TargetArchitecture.IA64);
			this.has_reloc = module.Architecture == TargetArchitecture.I386;
			this.GetDebugHeader();
			this.GetWin32Resources();
			this.text_map = this.BuildTextMap();
			this.sections = (ushort)((this.has_reloc ? 2 : 1));
			DateTime utcNow = DateTime.UtcNow;
			TimeSpan timeSpan = utcNow.Subtract(new DateTime(1970, 1, 1));
			this.time_stamp = (uint)timeSpan.TotalSeconds;
		}

		private static uint Align(uint value, uint align)
		{
			align--;
			return value + align & ~align;
		}

		private void BuildSections()
		{
			bool win32Resources = this.win32_resources > null;
			if (win32Resources)
			{
				this.sections = (ushort)(this.sections + 1);
			}
			this.text = this.CreateSection(".text", this.text_map.GetLength(), null);
			Section section = this.text;
			if (win32Resources)
			{
				this.rsrc = this.CreateSection(".rsrc", (uint)this.win32_resources.length, section);
				this.PatchWin32Resources(this.win32_resources);
				section = this.rsrc;
			}
			if (this.has_reloc)
			{
				this.reloc = this.CreateSection(".reloc", 12, section);
			}
		}

		private TextMap BuildTextMap()
		{
			TextMap textMap = this.metadata.text_map;
			textMap.AddMap(TextSegment.Code, this.metadata.code.length, (!this.pe64 ? 4 : 16));
			textMap.AddMap(TextSegment.Resources, this.metadata.resources.length, 8);
			textMap.AddMap(TextSegment.Data, this.metadata.data.length, 4);
			if (this.metadata.data.length > 0)
			{
				this.metadata.table_heap.FixupData(textMap.GetRVA(TextSegment.Data));
			}
			textMap.AddMap(TextSegment.StrongNameSignature, this.GetStrongNameLength(), 4);
			textMap.AddMap(TextSegment.MetadataHeader, this.GetMetadataHeaderLength());
			textMap.AddMap(TextSegment.TableHeap, this.metadata.table_heap.length, 4);
			textMap.AddMap(TextSegment.StringHeap, this.metadata.string_heap.length, 4);
			textMap.AddMap(TextSegment.UserStringHeap, (this.metadata.user_string_heap.IsEmpty ? 0 : this.metadata.user_string_heap.length), 4);
			textMap.AddMap(TextSegment.GuidHeap, 16);
			textMap.AddMap(TextSegment.BlobHeap, (this.metadata.blob_heap.IsEmpty ? 0 : this.metadata.blob_heap.length), 4);
			int length = 0;
			if (!this.debug_data.IsNullOrEmpty<byte>())
			{
				this.debug_directory.AddressOfRawData = (int)(textMap.GetNextRVA(TextSegment.BlobHeap) + 28);
				length = (int)this.debug_data.Length + 28;
			}
			textMap.AddMap(TextSegment.DebugDirectory, length, 4);
			if (!this.has_reloc)
			{
				uint nextRVA = textMap.GetNextRVA(TextSegment.DebugDirectory);
				textMap.AddMap(TextSegment.ImportDirectory, new Range(nextRVA, 0));
				textMap.AddMap(TextSegment.ImportHintNameTable, new Range(nextRVA, 0));
				textMap.AddMap(TextSegment.StartupStub, new Range(nextRVA, 0));
				return textMap;
			}
			uint num = textMap.GetNextRVA(TextSegment.DebugDirectory);
			uint num1 = num + 48;
			num1 = num1 + 15 & -16;
			uint num2 = num1 - num + 27;
			uint num3 = num + num2;
			num3 = (this.module.Architecture == TargetArchitecture.IA64 ? num3 + 15 & -16 : 2 + (num3 + 3 & -4));
			textMap.AddMap(TextSegment.ImportDirectory, new Range(num, num2));
			textMap.AddMap(TextSegment.ImportHintNameTable, new Range(num1, 0));
			textMap.AddMap(TextSegment.StartupStub, new Range(num3, this.GetStartupStubLength()));
			return textMap;
		}

		private Section CreateSection(string name, uint size, Section previous)
		{
			uint virtualAddress;
			Section section = new Section()
			{
				Name = name
			};
			if (previous != null)
			{
				virtualAddress = previous.VirtualAddress + ImageWriter.Align(previous.VirtualSize, 8192);
			}
			else
			{
				virtualAddress = 8192;
			}
			section.VirtualAddress = virtualAddress;
			section.VirtualSize = size;
			section.PointerToRawData = (previous != null ? previous.PointerToRawData + previous.SizeOfRawData : ImageWriter.Align(this.GetHeaderSize(), 512));
			section.SizeOfRawData = ImageWriter.Align(size, 512);
			return section;
		}

		public static ImageWriter CreateWriter(ModuleDefinition module, MetadataBuilder metadata, Stream stream)
		{
			ImageWriter imageWriter = new ImageWriter(module, metadata, stream);
			imageWriter.BuildSections();
			return imageWriter;
		}

		private void GetDebugHeader()
		{
			ISymbolWriter symbolWriter = this.metadata.symbol_writer;
			if (symbolWriter == null)
			{
				return;
			}
			if (!symbolWriter.GetDebugHeader(out this.debug_directory, out this.debug_data))
			{
				this.debug_data = Empty<byte>.Array;
			}
		}

		public uint GetHeaderSize()
		{
			return (uint)(152 + this.SizeOfOptionalHeader() + this.sections * 40);
		}

		private Section GetImageResourceSection()
		{
			if (!this.module.HasImage)
			{
				return null;
			}
			return this.module.Image.GetSection(".rsrc");
		}

		private ushort GetMachine()
		{
			switch (this.module.Architecture)
			{
				case TargetArchitecture.I386:
				{
					return (ushort)332;
				}
				case TargetArchitecture.AMD64:
				{
					return (ushort)34404;
				}
				case TargetArchitecture.IA64:
				{
					return (ushort)512;
				}
				case TargetArchitecture.ARMv7:
				{
					return (ushort)452;
				}
			}
			throw new NotSupportedException();
		}

		private int GetMetadataHeaderLength()
		{
			return 72 + (this.metadata.user_string_heap.IsEmpty ? 0 : 12) + 16 + (this.metadata.blob_heap.IsEmpty ? 0 : 16);
		}

		private uint GetMetadataLength()
		{
			return this.text_map.GetRVA(TextSegment.DebugDirectory) - this.text_map.GetRVA(TextSegment.MetadataHeader);
		}

		private byte[] GetRuntimeMain()
		{
			if (this.module.Kind != ModuleKind.Dll && this.module.Kind != ModuleKind.NetModule)
			{
				return ImageWriter.GetSimpleString("_CorExeMain");
			}
			return ImageWriter.GetSimpleString("_CorDllMain");
		}

		private static byte[] GetSimpleString(string @string)
		{
			return ImageWriter.GetString(@string, @string.Length);
		}

		private uint GetStartupStubLength()
		{
			if (this.module.Architecture != TargetArchitecture.I386)
			{
				throw new NotSupportedException();
			}
			return (uint)6;
		}

		private ushort GetStreamCount()
		{
			return (ushort)(2 + (this.metadata.user_string_heap.IsEmpty ? 0 : 1) + 1 + (this.metadata.blob_heap.IsEmpty ? 0 : 1));
		}

		private static byte[] GetString(string @string, int length)
		{
			byte[] str = new byte[length];
			for (int i = 0; i < @string.Length; i++)
			{
				str[i] = (byte)@string[i];
			}
			return str;
		}

		private int GetStrongNameLength()
		{
			if (this.module.Assembly == null)
			{
				return 0;
			}
			byte[] publicKey = this.module.Assembly.Name.PublicKey;
			if (publicKey.IsNullOrEmpty<byte>())
			{
				return 0;
			}
			int length = (int)publicKey.Length;
			if (length <= 32)
			{
				return 128;
			}
			return length - 32;
		}

		public DataDirectory GetStrongNameSignatureDirectory()
		{
			return this.text_map.GetDataDirectory(TextSegment.StrongNameSignature);
		}

		private ushort GetSubSystem()
		{
			switch (this.module.Kind)
			{
				case ModuleKind.Dll:
				case ModuleKind.Console:
				case ModuleKind.NetModule:
				{
					return (ushort)3;
				}
				case ModuleKind.Windows:
				{
					return (ushort)2;
				}
			}
			throw new ArgumentOutOfRangeException();
		}

		private void GetWin32Resources()
		{
			Section imageResourceSection = this.GetImageResourceSection();
			if (imageResourceSection == null)
			{
				return;
			}
			byte[] numArray = new byte[(int)imageResourceSection.Data.Length];
			Buffer.BlockCopy(imageResourceSection.Data, 0, numArray, 0, (int)imageResourceSection.Data.Length);
			this.win32_resources = new ByteBuffer(numArray);
		}

		private static byte[] GetZeroTerminatedString(string @string)
		{
			return ImageWriter.GetString(@string, @string.Length + 1 + 3 & -4);
		}

		private Section LastSection()
		{
			if (this.reloc != null)
			{
				return this.reloc;
			}
			if (this.rsrc != null)
			{
				return this.rsrc;
			}
			return this.text;
		}

		private void MoveTo(uint pointer)
		{
			this.BaseStream.Seek((long)pointer, SeekOrigin.Begin);
		}

		private void MoveToRVA(Section section, uint rva)
		{
			this.BaseStream.Seek((long)(section.PointerToRawData + rva - section.VirtualAddress), SeekOrigin.Begin);
		}

		private void MoveToRVA(TextSegment segment)
		{
			this.MoveToRVA(this.text, this.text_map.GetRVA(segment));
		}

		private void PatchResourceDataEntry(ByteBuffer resources)
		{
			Section imageResourceSection = this.GetImageResourceSection();
			uint num = resources.ReadUInt32();
			resources.position -= 4;
			resources.WriteUInt32(num - imageResourceSection.VirtualAddress + this.rsrc.VirtualAddress);
		}

		private void PatchResourceDirectoryEntry(ByteBuffer resources)
		{
			resources.Advance(4);
			uint num = resources.ReadUInt32();
			int num1 = resources.position;
			resources.position = (int)(num & 2147483647);
			if ((num & -2147483648) == 0)
			{
				this.PatchResourceDataEntry(resources);
			}
			else
			{
				this.PatchResourceDirectoryTable(resources);
			}
			resources.position = num1;
		}

		private void PatchResourceDirectoryTable(ByteBuffer resources)
		{
			resources.Advance(12);
			int num = resources.ReadUInt16() + resources.ReadUInt16();
			for (int i = 0; i < num; i++)
			{
				this.PatchResourceDirectoryEntry(resources);
			}
		}

		private void PatchWin32Resources(ByteBuffer resources)
		{
			this.PatchResourceDirectoryTable(resources);
		}

		private void PrepareSection(Section section)
		{
			this.MoveTo(section.PointerToRawData);
			if (section.SizeOfRawData <= 4096)
			{
				this.Write(new byte[section.SizeOfRawData]);
				this.MoveTo(section.PointerToRawData);
				return;
			}
			int num = 0;
			byte[] numArray = new byte[4096];
			while ((long)num != (ulong)section.SizeOfRawData)
			{
				int num1 = System.Math.Min((int)(section.SizeOfRawData - num), 4096);
				this.Write(numArray, 0, num1);
				num += num1;
			}
			this.MoveTo(section.PointerToRawData);
		}

		private ushort SizeOfOptionalHeader()
		{
			return (ushort)((!this.pe64 ? 224 : 240));
		}

		private void WriteDebugDirectory()
		{
			base.WriteInt32(this.debug_directory.Characteristics);
			base.WriteUInt32(this.time_stamp);
			base.WriteInt16(this.debug_directory.MajorVersion);
			base.WriteInt16(this.debug_directory.MinorVersion);
			base.WriteInt32(this.debug_directory.Type);
			base.WriteInt32(this.debug_directory.SizeOfData);
			base.WriteInt32(this.debug_directory.AddressOfRawData);
			base.WriteInt32((int)this.BaseStream.Position + 4);
			base.WriteBytes(this.debug_data);
		}

		private void WriteDOSHeader()
		{
			this.Write(new byte[] { 77, 90, 144, 0, 3, 0, 0, 0, 4, 0, 0, 0, 255, 255, 0, 0, 184, 0, 0, 0, 0, 0, 0, 0, 64, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 128, 0, 0, 0, 14, 31, 186, 14, 0, 180, 9, 205, 33, 184, 1, 76, 205, 33, 84, 104, 105, 115, 32, 112, 114, 111, 103, 114, 97, 109, 32, 99, 97, 110, 110, 111, 116, 32, 98, 101, 32, 114, 117, 110, 32, 105, 110, 32, 68, 79, 83, 32, 109, 111, 100, 101, 46, 13, 13, 10, 36, 0, 0, 0, 0, 0, 0, 0 });
		}

		private void WriteGuidHeap()
		{
			this.MoveToRVA(TextSegment.GuidHeap);
			base.WriteBytes(this.module.Mvid.ToByteArray());
		}

		private void WriteHeap(TextSegment heap, HeapBuffer buffer)
		{
			if (buffer.IsEmpty)
			{
				return;
			}
			this.MoveToRVA(heap);
			base.WriteBuffer(buffer);
		}

		public void WriteImage()
		{
			this.WriteDOSHeader();
			this.WritePEFileHeader();
			this.WriteOptionalHeaders();
			this.WriteSectionHeaders();
			this.WriteText();
			if (this.rsrc != null)
			{
				this.WriteRsrc();
			}
			if (this.reloc != null)
			{
				this.WriteReloc();
			}
		}

		private void WriteImportDirectory()
		{
			base.WriteUInt32(this.text_map.GetRVA(TextSegment.ImportDirectory) + 40);
			base.WriteUInt32(0);
			base.WriteUInt32(0);
			base.WriteUInt32(this.text_map.GetRVA(TextSegment.ImportHintNameTable) + 14);
			base.WriteUInt32(this.text_map.GetRVA(TextSegment.ImportAddressTable));
			base.Advance(20);
			base.WriteUInt32(this.text_map.GetRVA(TextSegment.ImportHintNameTable));
			this.MoveToRVA(TextSegment.ImportHintNameTable);
			base.WriteUInt16(0);
			base.WriteBytes(this.GetRuntimeMain());
			base.WriteByte(0);
			base.WriteBytes(ImageWriter.GetSimpleString("mscoree.dll"));
			base.WriteUInt16(0);
		}

		private void WriteMetadata()
		{
			this.WriteHeap(TextSegment.TableHeap, this.metadata.table_heap);
			this.WriteHeap(TextSegment.StringHeap, this.metadata.string_heap);
			this.WriteHeap(TextSegment.UserStringHeap, this.metadata.user_string_heap);
			this.WriteGuidHeap();
			this.WriteHeap(TextSegment.BlobHeap, this.metadata.blob_heap);
		}

		private void WriteMetadataHeader()
		{
			base.WriteUInt32(1112167234);
			base.WriteUInt16(1);
			base.WriteUInt16(1);
			base.WriteUInt32(0);
			byte[] zeroTerminatedString = ImageWriter.GetZeroTerminatedString(this.module.runtime_version);
			base.WriteUInt32((uint)zeroTerminatedString.Length);
			base.WriteBytes(zeroTerminatedString);
			base.WriteUInt16(0);
			base.WriteUInt16(this.GetStreamCount());
			uint rVA = this.text_map.GetRVA(TextSegment.TableHeap) - this.text_map.GetRVA(TextSegment.MetadataHeader);
			this.WriteStreamHeader(ref rVA, TextSegment.TableHeap, "#~");
			this.WriteStreamHeader(ref rVA, TextSegment.StringHeap, "#Strings");
			this.WriteStreamHeader(ref rVA, TextSegment.UserStringHeap, "#US");
			this.WriteStreamHeader(ref rVA, TextSegment.GuidHeap, "#GUID");
			this.WriteStreamHeader(ref rVA, TextSegment.BlobHeap, "#Blob");
		}

		private void WriteOptionalHeaders()
		{
			Object sizeOfRawData;
			Object obj;
			uint start;
			uint virtualAddress;
			uint virtualSize;
			base.WriteUInt16((ushort)((!this.pe64 ? 267 : 523)));
			base.WriteByte(8);
			base.WriteByte(0);
			base.WriteUInt32(this.text.SizeOfRawData);
			if (this.reloc != null)
			{
				sizeOfRawData = this.reloc.SizeOfRawData;
			}
			else
			{
				sizeOfRawData = null;
			}
			if (this.rsrc != null)
			{
				obj = this.rsrc.SizeOfRawData;
			}
			else
			{
				obj = null;
			}
			base.WriteUInt32(sizeOfRawData + obj);
			base.WriteUInt32(0);
			Range range = this.text_map.GetRange(TextSegment.StartupStub);
			if (range.Length > 0)
			{
				start = range.Start;
			}
			else
			{
				start = 0;
			}
			base.WriteUInt32(start);
			base.WriteUInt32(8192);
			if (this.pe64)
			{
				base.WriteUInt64((ulong)4194304);
			}
			else
			{
				base.WriteUInt32(0);
				base.WriteUInt32(4194304);
			}
			base.WriteUInt32(8192);
			base.WriteUInt32(512);
			base.WriteUInt16(4);
			base.WriteUInt16(0);
			base.WriteUInt16(0);
			base.WriteUInt16(0);
			base.WriteUInt16(4);
			base.WriteUInt16(0);
			base.WriteUInt32(0);
			Section section = this.LastSection();
			base.WriteUInt32(section.VirtualAddress + ImageWriter.Align(section.VirtualSize, 8192));
			base.WriteUInt32(this.text.PointerToRawData);
			base.WriteUInt32(0);
			base.WriteUInt16(this.GetSubSystem());
			base.WriteUInt16((ushort)this.module.Characteristics);
			if (this.pe64)
			{
				base.WriteUInt64((ulong)1048576);
				base.WriteUInt64((ulong)4096);
				base.WriteUInt64((ulong)1048576);
				base.WriteUInt64((ulong)4096);
			}
			else
			{
				base.WriteUInt32(1048576);
				base.WriteUInt32(4096);
				base.WriteUInt32(1048576);
				base.WriteUInt32(4096);
			}
			base.WriteUInt32(0);
			base.WriteUInt32(16);
			this.WriteZeroDataDirectory();
			base.WriteDataDirectory(this.text_map.GetDataDirectory(TextSegment.ImportDirectory));
			if (this.rsrc == null)
			{
				this.WriteZeroDataDirectory();
			}
			else
			{
				base.WriteUInt32(this.rsrc.VirtualAddress);
				base.WriteUInt32(this.rsrc.VirtualSize);
			}
			this.WriteZeroDataDirectory();
			this.WriteZeroDataDirectory();
			if (this.reloc != null)
			{
				virtualAddress = this.reloc.VirtualAddress;
			}
			else
			{
				virtualAddress = 0;
			}
			base.WriteUInt32(virtualAddress);
			if (this.reloc != null)
			{
				virtualSize = this.reloc.VirtualSize;
			}
			else
			{
				virtualSize = 0;
			}
			base.WriteUInt32(virtualSize);
			if (this.text_map.GetLength(TextSegment.DebugDirectory) <= 0)
			{
				this.WriteZeroDataDirectory();
			}
			else
			{
				base.WriteUInt32(this.text_map.GetRVA(TextSegment.DebugDirectory));
				base.WriteUInt32(28);
			}
			this.WriteZeroDataDirectory();
			this.WriteZeroDataDirectory();
			this.WriteZeroDataDirectory();
			this.WriteZeroDataDirectory();
			this.WriteZeroDataDirectory();
			base.WriteDataDirectory(this.text_map.GetDataDirectory(TextSegment.ImportAddressTable));
			this.WriteZeroDataDirectory();
			base.WriteDataDirectory(this.text_map.GetDataDirectory(TextSegment.CLIHeader));
			this.WriteZeroDataDirectory();
		}

		private void WritePEFileHeader()
		{
			base.WriteUInt32(17744);
			base.WriteUInt16(this.GetMachine());
			base.WriteUInt16(this.sections);
			base.WriteUInt32(this.time_stamp);
			base.WriteUInt32(0);
			base.WriteUInt32(0);
			base.WriteUInt16(this.SizeOfOptionalHeader());
			ushort num = (ushort)(2 | (!this.pe64 ? 256 : 32));
			if (this.module.Kind == ModuleKind.Dll || this.module.Kind == ModuleKind.NetModule)
			{
				num = (ushort)(num | 8192);
			}
			base.WriteUInt16(num);
		}

		private void WriteReloc()
		{
			this.PrepareSection(this.reloc);
			uint rVA = this.text_map.GetRVA(TextSegment.StartupStub);
			rVA = rVA + (this.module.Architecture == TargetArchitecture.IA64 ? 32 : 2);
			uint num = rVA & -4096;
			base.WriteUInt32(num);
			base.WriteUInt32(12);
			if (this.module.Architecture != TargetArchitecture.I386)
			{
				throw new NotSupportedException();
			}
			base.WriteUInt32(12288 + rVA - num);
		}

		private void WriteRsrc()
		{
			this.PrepareSection(this.rsrc);
			base.WriteBuffer(this.win32_resources);
		}

		private void WriteRVA(uint rva)
		{
			if (!this.pe64)
			{
				base.WriteUInt32(rva);
				return;
			}
			base.WriteUInt64((ulong)rva);
		}

		private void WriteSection(Section section, uint characteristics)
		{
			byte[] numArray = new byte[8];
			string name = section.Name;
			for (int i = 0; i < name.Length; i++)
			{
				numArray[i] = (byte)name[i];
			}
			base.WriteBytes(numArray);
			base.WriteUInt32(section.VirtualSize);
			base.WriteUInt32(section.VirtualAddress);
			base.WriteUInt32(section.SizeOfRawData);
			base.WriteUInt32(section.PointerToRawData);
			base.WriteUInt32(0);
			base.WriteUInt32(0);
			base.WriteUInt16(0);
			base.WriteUInt16(0);
			base.WriteUInt32(characteristics);
		}

		private void WriteSectionHeaders()
		{
			this.WriteSection(this.text, 1610612768);
			if (this.rsrc != null)
			{
				this.WriteSection(this.rsrc, 1073741888);
			}
			if (this.reloc != null)
			{
				this.WriteSection(this.reloc, 1107296320);
			}
		}

		private void WriteStartupStub()
		{
			if (this.module.Architecture != TargetArchitecture.I386)
			{
				throw new NotSupportedException();
			}
			base.WriteUInt16(9727);
			base.WriteUInt32(4194304 + this.text_map.GetRVA(TextSegment.ImportAddressTable));
		}

		private void WriteStreamHeader(ref uint offset, TextSegment heap, string name)
		{
			uint length = (uint)this.text_map.GetLength(heap);
			if (length == 0)
			{
				return;
			}
			base.WriteUInt32(offset);
			base.WriteUInt32(length);
			base.WriteBytes(ImageWriter.GetZeroTerminatedString(name));
			offset += length;
		}

		private void WriteText()
		{
			object obj;
			this.PrepareSection(this.text);
			if (this.has_reloc)
			{
				this.WriteRVA(this.text_map.GetRVA(TextSegment.ImportHintNameTable));
				this.WriteRVA(0);
			}
			base.WriteUInt32(72);
			base.WriteUInt16(2);
			if (this.module.Runtime <= TargetRuntime.Net_1_1)
			{
				obj = null;
			}
			else
			{
				obj = 5;
			}
			base.WriteUInt16((ushort)obj);
			base.WriteUInt32(this.text_map.GetRVA(TextSegment.MetadataHeader));
			base.WriteUInt32(this.GetMetadataLength());
			base.WriteUInt32((uint)this.module.Attributes);
			base.WriteUInt32(this.metadata.entry_point.ToUInt32());
			base.WriteDataDirectory(this.text_map.GetDataDirectory(TextSegment.Resources));
			base.WriteDataDirectory(this.text_map.GetDataDirectory(TextSegment.StrongNameSignature));
			this.WriteZeroDataDirectory();
			this.WriteZeroDataDirectory();
			this.WriteZeroDataDirectory();
			this.WriteZeroDataDirectory();
			this.MoveToRVA(TextSegment.Code);
			base.WriteBuffer(this.metadata.code);
			this.MoveToRVA(TextSegment.Resources);
			base.WriteBuffer(this.metadata.resources);
			if (this.metadata.data.length > 0)
			{
				this.MoveToRVA(TextSegment.Data);
				base.WriteBuffer(this.metadata.data);
			}
			this.MoveToRVA(TextSegment.MetadataHeader);
			this.WriteMetadataHeader();
			this.WriteMetadata();
			if (this.text_map.GetLength(TextSegment.DebugDirectory) > 0)
			{
				this.MoveToRVA(TextSegment.DebugDirectory);
				this.WriteDebugDirectory();
			}
			if (!this.has_reloc)
			{
				return;
			}
			this.MoveToRVA(TextSegment.ImportDirectory);
			this.WriteImportDirectory();
			this.MoveToRVA(TextSegment.StartupStub);
			this.WriteStartupStub();
		}

		private void WriteZeroDataDirectory()
		{
			base.WriteUInt32(0);
			base.WriteUInt32(0);
		}
	}
}