using Mono.Cecil;
using Mono.Cecil.Metadata;
using System;
using System.IO;

namespace Mono.Cecil.PE
{
	internal sealed class ImageReader : BinaryStreamReader
	{
		private readonly Image image;

		private DataDirectory cli;

		private DataDirectory metadata;

		public ImageReader(Stream stream) : base(stream)
		{
			this.image = new Image()
			{
				FileName = stream.GetFullyQualifiedName()
			};
		}

		private void ComputeTableInformations()
		{
			int indexSize;
			uint position = (uint)this.BaseStream.Position - this.image.MetadataSection.PointerToRawData;
			int num = this.image.StringHeap.IndexSize;
			int num1 = (this.image.BlobHeap != null ? this.image.BlobHeap.IndexSize : 2);
			TableHeap tableHeap = this.image.TableHeap;
			TableInformation[] tables = tableHeap.Tables;
			for (int i = 0; i < 45; i++)
			{
				Table table = (Table)((byte)i);
				if (tableHeap.HasTable(table))
				{
					switch (table)
					{
						case Table.Module:
						{
							indexSize = 2 + num + this.image.GuidHeap.IndexSize * 3;
							break;
						}
						case Table.TypeRef:
						{
							indexSize = this.GetCodedIndexSize(CodedIndex.ResolutionScope) + num * 2;
							break;
						}
						case Table.TypeDef:
						{
							indexSize = 4 + num * 2 + this.GetCodedIndexSize(CodedIndex.TypeDefOrRef) + this.GetTableIndexSize(Table.Field) + this.GetTableIndexSize(Table.Method);
							break;
						}
						case Table.FieldPtr:
						{
							indexSize = this.GetTableIndexSize(Table.Field);
							break;
						}
						case Table.Field:
						{
							indexSize = 2 + num + num1;
							break;
						}
						case Table.MethodPtr:
						{
							indexSize = this.GetTableIndexSize(Table.Method);
							break;
						}
						case Table.Method:
						{
							indexSize = 8 + num + num1 + this.GetTableIndexSize(Table.Param);
							break;
						}
						case Table.ParamPtr:
						{
							indexSize = this.GetTableIndexSize(Table.Param);
							break;
						}
						case Table.Param:
						{
							indexSize = 4 + num;
							break;
						}
						case Table.InterfaceImpl:
						{
							indexSize = this.GetTableIndexSize(Table.TypeDef) + this.GetCodedIndexSize(CodedIndex.TypeDefOrRef);
							break;
						}
						case Table.MemberRef:
						{
							indexSize = this.GetCodedIndexSize(CodedIndex.MemberRefParent) + num + num1;
							break;
						}
						case Table.Constant:
						{
							indexSize = 2 + this.GetCodedIndexSize(CodedIndex.HasConstant) + num1;
							break;
						}
						case Table.CustomAttribute:
						{
							indexSize = this.GetCodedIndexSize(CodedIndex.HasCustomAttribute) + this.GetCodedIndexSize(CodedIndex.CustomAttributeType) + num1;
							break;
						}
						case Table.FieldMarshal:
						{
							indexSize = this.GetCodedIndexSize(CodedIndex.HasFieldMarshal) + num1;
							break;
						}
						case Table.DeclSecurity:
						{
							indexSize = 2 + this.GetCodedIndexSize(CodedIndex.HasDeclSecurity) + num1;
							break;
						}
						case Table.ClassLayout:
						{
							indexSize = 6 + this.GetTableIndexSize(Table.TypeDef);
							break;
						}
						case Table.FieldLayout:
						{
							indexSize = 4 + this.GetTableIndexSize(Table.Field);
							break;
						}
						case Table.StandAloneSig:
						{
							indexSize = num1;
							break;
						}
						case Table.EventMap:
						{
							indexSize = this.GetTableIndexSize(Table.TypeDef) + this.GetTableIndexSize(Table.Event);
							break;
						}
						case Table.EventPtr:
						{
							indexSize = this.GetTableIndexSize(Table.Event);
							break;
						}
						case Table.Event:
						{
							indexSize = 2 + num + this.GetCodedIndexSize(CodedIndex.TypeDefOrRef);
							break;
						}
						case Table.PropertyMap:
						{
							indexSize = this.GetTableIndexSize(Table.TypeDef) + this.GetTableIndexSize(Table.Property);
							break;
						}
						case Table.PropertyPtr:
						{
							indexSize = this.GetTableIndexSize(Table.Property);
							break;
						}
						case Table.Property:
						{
							indexSize = 2 + num + num1;
							break;
						}
						case Table.MethodSemantics:
						{
							indexSize = 2 + this.GetTableIndexSize(Table.Method) + this.GetCodedIndexSize(CodedIndex.HasSemantics);
							break;
						}
						case Table.MethodImpl:
						{
							indexSize = this.GetTableIndexSize(Table.TypeDef) + this.GetCodedIndexSize(CodedIndex.MethodDefOrRef) + this.GetCodedIndexSize(CodedIndex.MethodDefOrRef);
							break;
						}
						case Table.ModuleRef:
						{
							indexSize = num;
							break;
						}
						case Table.TypeSpec:
						{
							indexSize = num1;
							break;
						}
						case Table.ImplMap:
						{
							indexSize = 2 + this.GetCodedIndexSize(CodedIndex.MemberForwarded) + num + this.GetTableIndexSize(Table.ModuleRef);
							break;
						}
						case Table.FieldRVA:
						{
							indexSize = 4 + this.GetTableIndexSize(Table.Field);
							break;
						}
						case Table.EncLog:
						{
							indexSize = 8;
							break;
						}
						case Table.EncMap:
						{
							indexSize = 4;
							break;
						}
						case Table.Assembly:
						{
							indexSize = 16 + num1 + num * 2;
							break;
						}
						case Table.AssemblyProcessor:
						{
							indexSize = 4;
							break;
						}
						case Table.AssemblyOS:
						{
							indexSize = 12;
							break;
						}
						case Table.AssemblyRef:
						{
							indexSize = 12 + num1 * 2 + num * 2;
							break;
						}
						case Table.AssemblyRefProcessor:
						{
							indexSize = 4 + this.GetTableIndexSize(Table.AssemblyRef);
							break;
						}
						case Table.AssemblyRefOS:
						{
							indexSize = 12 + this.GetTableIndexSize(Table.AssemblyRef);
							break;
						}
						case Table.File:
						{
							indexSize = 4 + num + num1;
							break;
						}
						case Table.ExportedType:
						{
							indexSize = 8 + num * 2 + this.GetCodedIndexSize(CodedIndex.Implementation);
							break;
						}
						case Table.ManifestResource:
						{
							indexSize = 8 + num + this.GetCodedIndexSize(CodedIndex.Implementation);
							break;
						}
						case Table.NestedClass:
						{
							indexSize = this.GetTableIndexSize(Table.TypeDef) + this.GetTableIndexSize(Table.TypeDef);
							break;
						}
						case Table.GenericParam:
						{
							indexSize = 4 + this.GetCodedIndexSize(CodedIndex.TypeOrMethodDef) + num;
							break;
						}
						case Table.MethodSpec:
						{
							indexSize = this.GetCodedIndexSize(CodedIndex.MethodDefOrRef) + num1;
							break;
						}
						case Table.GenericParamConstraint:
						{
							indexSize = this.GetTableIndexSize(Table.GenericParam) + this.GetCodedIndexSize(CodedIndex.TypeDefOrRef);
							break;
						}
						default:
						{
							throw new NotSupportedException();
						}
					}
					tables[i].RowSize = (uint)indexSize;
					tables[i].Offset = position;
					position = position + indexSize * tables[i].Length;
				}
			}
		}

		private int GetCodedIndexSize(CodedIndex index)
		{
			return this.image.GetCodedIndexSize(index);
		}

		private static ModuleKind GetModuleKind(ushort characteristics, ushort subsystem)
		{
			if ((characteristics & 8192) != 0)
			{
				return ModuleKind.Dll;
			}
			if (subsystem != 2 && subsystem != 9)
			{
				return ModuleKind.Console;
			}
			return ModuleKind.Windows;
		}

		private int GetTableIndexSize(Table table)
		{
			return this.image.GetTableIndexSize(table);
		}

		private void MoveTo(DataDirectory directory)
		{
			this.BaseStream.Position = (long)this.image.ResolveVirtualAddress(directory.VirtualAddress);
		}

		private void MoveTo(uint position)
		{
			this.BaseStream.Position = (long)position;
		}

		private string ReadAlignedString(int length)
		{
			int num = 0;
			char[] chrArray = new char[length];
			while (num < length)
			{
				byte num1 = this.ReadByte();
				if (num1 == 0)
				{
					break;
				}
				int num2 = num;
				num = num2 + 1;
				chrArray[num2] = (char)num1;
			}
			base.Advance(-1 + (num + 4 & -4) - num);
			return new string(chrArray, 0, num);
		}

		private TargetArchitecture ReadArchitecture()
		{
			ushort num = this.ReadUInt16();
			if (num > 452)
			{
				if (num == 512)
				{
					return TargetArchitecture.IA64;
				}
				if (num == 34404)
				{
					return TargetArchitecture.AMD64;
				}
			}
			else
			{
				if (num == 332)
				{
					return TargetArchitecture.I386;
				}
				if (num == 452)
				{
					return TargetArchitecture.ARMv7;
				}
			}
			throw new NotSupportedException();
		}

		private void ReadCLIHeader()
		{
			this.MoveTo(this.cli);
			base.Advance(8);
			this.metadata = base.ReadDataDirectory();
			this.image.Attributes = (ModuleAttributes)this.ReadUInt32();
			this.image.EntryPointToken = this.ReadUInt32();
			this.image.Resources = base.ReadDataDirectory();
			this.image.StrongName = base.ReadDataDirectory();
		}

		private void ReadImage()
		{
			ushort num;
			ushort num1;
			if (this.BaseStream.Length < (long)128)
			{
				throw new BadImageFormatException();
			}
			if (this.ReadUInt16() != 23117)
			{
				throw new BadImageFormatException();
			}
			base.Advance(58);
			this.MoveTo(this.ReadUInt32());
			if (this.ReadUInt32() != 17744)
			{
				throw new BadImageFormatException();
			}
			this.image.Architecture = this.ReadArchitecture();
			ushort num2 = this.ReadUInt16();
			base.Advance(14);
			ushort num3 = this.ReadUInt16();
			this.ReadOptionalHeaders(out num, out num1);
			this.ReadSections(num2);
			this.ReadCLIHeader();
			this.ReadMetadata();
			this.image.Kind = ImageReader.GetModuleKind(num3, num);
			this.image.Characteristics = (ModuleCharacteristics)num1;
		}

		public static Image ReadImageFrom(Stream stream)
		{
			Image image;
			try
			{
				ImageReader imageReader = new ImageReader(stream);
				imageReader.ReadImage();
				image = imageReader.image;
			}
			catch (EndOfStreamException endOfStreamException)
			{
				throw new BadImageFormatException(stream.GetFullyQualifiedName(), endOfStreamException);
			}
			return image;
		}

		private void ReadMetadata()
		{
			this.MoveTo(this.metadata);
			if (this.ReadUInt32() != 1112167234)
			{
				throw new BadImageFormatException();
			}
			base.Advance(8);
			this.image.RuntimeVersion = this.ReadZeroTerminatedString(this.ReadInt32());
			base.Advance(2);
			ushort num = this.ReadUInt16();
			Section sectionAtVirtualAddress = this.image.GetSectionAtVirtualAddress(this.metadata.VirtualAddress);
			if (sectionAtVirtualAddress == null)
			{
				throw new BadImageFormatException();
			}
			this.image.MetadataSection = sectionAtVirtualAddress;
			for (int i = 0; i < num; i++)
			{
				this.ReadMetadataStream(sectionAtVirtualAddress);
			}
			if (this.image.TableHeap != null)
			{
				this.ReadTableHeap();
			}
		}

		private void ReadMetadataStream(Section section)
		{
			uint virtualAddress = this.metadata.VirtualAddress - section.VirtualAddress + this.ReadUInt32();
			uint num = this.ReadUInt32();
			string str = this.ReadAlignedString(16);
			if (str == "#~" || str == "#-")
			{
				this.image.TableHeap = new TableHeap(section, virtualAddress, num);
				return;
			}
			if (str == "#Strings")
			{
				this.image.StringHeap = new StringHeap(section, virtualAddress, num);
				return;
			}
			if (str == "#Blob")
			{
				this.image.BlobHeap = new BlobHeap(section, virtualAddress, num);
				return;
			}
			if (str == "#GUID")
			{
				this.image.GuidHeap = new GuidHeap(section, virtualAddress, num);
				return;
			}
			if (str != "#US")
			{
				return;
			}
			this.image.UserStringHeap = new UserStringHeap(section, virtualAddress, num);
		}

		private void ReadOptionalHeaders(out ushort subsystem, out ushort dll_characteristics)
		{
			bool flag = this.ReadUInt16() == 523;
			base.Advance(66);
			subsystem = this.ReadUInt16();
			dll_characteristics = this.ReadUInt16();
			base.Advance((flag ? 88 : 72));
			this.image.Debug = base.ReadDataDirectory();
			base.Advance(56);
			this.cli = base.ReadDataDirectory();
			if (this.cli.IsZero)
			{
				throw new BadImageFormatException();
			}
			base.Advance(8);
		}

		private void ReadSectionData(Section section)
		{
			long position = this.BaseStream.Position;
			this.MoveTo(section.PointerToRawData);
			int sizeOfRawData = (int)section.SizeOfRawData;
			byte[] numArray = new byte[sizeOfRawData];
			int num = 0;
			while (true)
			{
				int num1 = this.Read(numArray, num, sizeOfRawData - num);
				int num2 = num1;
				if (num1 <= 0)
				{
					break;
				}
				num += num2;
			}
			section.Data = numArray;
			this.BaseStream.Position = position;
		}

		private void ReadSections(ushort count)
		{
			Section[] sectionArray = new Section[count];
			for (int i = 0; i < count; i++)
			{
				Section section = new Section()
				{
					Name = this.ReadZeroTerminatedString(8)
				};
				base.Advance(4);
				section.VirtualAddress = this.ReadUInt32();
				section.SizeOfRawData = this.ReadUInt32();
				section.PointerToRawData = this.ReadUInt32();
				base.Advance(16);
				sectionArray[i] = section;
				this.ReadSectionData(section);
			}
			this.image.Sections = sectionArray;
		}

		private void ReadTableHeap()
		{
			TableHeap tableHeap = this.image.TableHeap;
			uint pointerToRawData = tableHeap.Section.PointerToRawData;
			this.MoveTo(tableHeap.Offset + pointerToRawData);
			base.Advance(6);
			byte num = this.ReadByte();
			base.Advance(1);
			tableHeap.Valid = this.ReadInt64();
			tableHeap.Sorted = this.ReadInt64();
			for (int i = 0; i < 45; i++)
			{
				if (tableHeap.HasTable((Table)((byte)i)))
				{
					tableHeap.Tables[i].Length = this.ReadUInt32();
				}
			}
			ImageReader.SetIndexSize(this.image.StringHeap, num, 1);
			ImageReader.SetIndexSize(this.image.GuidHeap, num, 2);
			ImageReader.SetIndexSize(this.image.BlobHeap, num, 4);
			this.ComputeTableInformations();
		}

		private string ReadZeroTerminatedString(int length)
		{
			int num = 0;
			char[] chrArray = new char[length];
			byte[] numArray = this.ReadBytes(length);
			while (num < length)
			{
				byte num1 = numArray[num];
				if (num1 == 0)
				{
					break;
				}
				int num2 = num;
				num = num2 + 1;
				chrArray[num2] = (char)num1;
			}
			return new string(chrArray, 0, num);
		}

		private static void SetIndexSize(Heap heap, uint sizes, byte flag)
		{
			if (heap == null)
			{
				return;
			}
			heap.IndexSize = ((sizes & flag) > 0 ? 4 : 2);
		}
	}
}