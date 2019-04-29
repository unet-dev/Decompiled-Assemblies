using Mono;
using Mono.Cecil.Cil;
using Mono.Cecil.Metadata;
using Mono.Cecil.PE;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace Mono.Cecil
{
	internal sealed class MetadataReader : ByteBuffer
	{
		internal readonly Image image;

		internal readonly ModuleDefinition module;

		internal readonly MetadataSystem metadata;

		internal IGenericContext context;

		internal CodeReader code;

		private uint Position
		{
			get
			{
				return (uint)this.position;
			}
			set
			{
				this.position = (int)value;
			}
		}

		public MetadataReader(ModuleDefinition module) : base(module.Image.MetadataSection.Data)
		{
			this.image = module.Image;
			this.module = module;
			this.metadata = module.MetadataSystem;
			this.code = new CodeReader(this.image.MetadataSection, this);
		}

		private void AddGenericConstraintMapping(uint generic_parameter, MetadataToken constraint)
		{
			this.metadata.SetGenericConstraintMapping(generic_parameter, MetadataReader.AddMapping<uint, MetadataToken>(this.metadata.GenericConstraints, generic_parameter, constraint));
		}

		private void AddInterfaceMapping(uint type, MetadataToken @interface)
		{
			this.metadata.SetInterfaceMapping(type, MetadataReader.AddMapping<uint, MetadataToken>(this.metadata.Interfaces, type, @interface));
		}

		private static TValue[] AddMapping<TKey, TValue>(Dictionary<TKey, TValue[]> cache, TKey key, TValue value)
		{
			TValue[] tValueArray;
			if (!cache.TryGetValue(key, out tValueArray))
			{
				tValueArray = new TValue[] { value };
				return tValueArray;
			}
			TValue[] tValueArray1 = new TValue[(int)tValueArray.Length + 1];
			Array.Copy(tValueArray, tValueArray1, (int)tValueArray.Length);
			tValueArray1[(int)tValueArray.Length] = value;
			return tValueArray1;
		}

		private void AddNestedMapping(uint declaring, uint nested)
		{
			this.metadata.SetNestedTypeMapping(declaring, MetadataReader.AddMapping<uint, uint>(this.metadata.NestedTypes, declaring, nested));
			this.metadata.SetReverseNestedTypeMapping(nested, declaring);
		}

		private void AddOverrideMapping(uint method_rid, MetadataToken @override)
		{
			this.metadata.SetOverrideMapping(method_rid, MetadataReader.AddMapping<uint, MetadataToken>(this.metadata.Overrides, method_rid, @override));
		}

		private static void AddRange(Dictionary<MetadataToken, Range[]> ranges, MetadataToken owner, Range range)
		{
			Range[] rangeArray;
			if (owner.RID == 0)
			{
				return;
			}
			if (!ranges.TryGetValue(owner, out rangeArray))
			{
				ranges.Add(owner, new Range[] { range });
				return;
			}
			rangeArray = rangeArray.Resize<Range>((int)rangeArray.Length + 1);
			rangeArray[(int)rangeArray.Length - 1] = range;
			ranges[owner] = rangeArray;
		}

		private void CompleteTypes()
		{
			TypeDefinition[] types = this.metadata.Types;
			for (int i = 0; i < (int)types.Length; i++)
			{
				TypeDefinition typeDefinition = types[i];
				MetadataReader.InitializeCollection(typeDefinition.Fields);
				MetadataReader.InitializeCollection(typeDefinition.Methods);
			}
		}

		private int GetCodedIndexSize(CodedIndex index)
		{
			return this.image.GetCodedIndexSize(index);
		}

		private static EventDefinition GetEvent(TypeDefinition type, MetadataToken token)
		{
			if (token.TokenType != Mono.Cecil.TokenType.Event)
			{
				throw new ArgumentException();
			}
			return MetadataReader.GetMember<EventDefinition>(type.Events, token);
		}

		private IMetadataScope GetExportedTypeScope(MetadataToken token)
		{
			IMetadataScope assemblyReferences;
			int num = this.position;
			Mono.Cecil.TokenType tokenType = token.TokenType;
			if (tokenType == Mono.Cecil.TokenType.AssemblyRef)
			{
				this.InitializeAssemblyReferences();
				assemblyReferences = this.metadata.AssemblyReferences[token.RID - 1];
			}
			else
			{
				if (tokenType != Mono.Cecil.TokenType.File)
				{
					throw new NotSupportedException();
				}
				this.InitializeModuleReferences();
				assemblyReferences = this.GetModuleReferenceFromFile(token);
			}
			this.position = num;
			return assemblyReferences;
		}

		public FieldDefinition GetFieldDefinition(uint rid)
		{
			this.InitializeTypeDefinitions();
			FieldDefinition fieldDefinition = this.metadata.GetFieldDefinition(rid);
			if (fieldDefinition != null)
			{
				return fieldDefinition;
			}
			return this.LookupField(rid);
		}

		private byte[] GetFieldInitializeValue(int size, uint rva)
		{
			Section sectionAtVirtualAddress = this.image.GetSectionAtVirtualAddress(rva);
			if (sectionAtVirtualAddress == null)
			{
				return Empty<byte>.Array;
			}
			byte[] numArray = new byte[size];
			Buffer.BlockCopy(sectionAtVirtualAddress.Data, (int)(rva - sectionAtVirtualAddress.VirtualAddress), numArray, 0, size);
			return numArray;
		}

		private static int GetFieldTypeSize(TypeReference type)
		{
			int classSize = 0;
			switch (type.etype)
			{
				case ElementType.Boolean:
				case ElementType.I1:
				case ElementType.U1:
				{
					classSize = 1;
					break;
				}
				case ElementType.Char:
				case ElementType.I2:
				case ElementType.U2:
				{
					classSize = 2;
					break;
				}
				case ElementType.I4:
				case ElementType.U4:
				case ElementType.R4:
				{
					classSize = 4;
					break;
				}
				case ElementType.I8:
				case ElementType.U8:
				case ElementType.R8:
				{
					classSize = 8;
					break;
				}
				case ElementType.String:
				case ElementType.ByRef:
				case ElementType.ValueType:
				case ElementType.Class:
				case ElementType.Var:
				case ElementType.Array:
				case ElementType.GenericInst:
				case ElementType.TypedByRef:
				case ElementType.Void | ElementType.Boolean | ElementType.Char | ElementType.I1 | ElementType.U1 | ElementType.I2 | ElementType.U2 | ElementType.ByRef | ElementType.ValueType | ElementType.Class | ElementType.Var | ElementType.Array | ElementType.GenericInst | ElementType.TypedByRef:
				case ElementType.I:
				case ElementType.U:
				case ElementType.Boolean | ElementType.I4 | ElementType.I8 | ElementType.ByRef | ElementType.Class | ElementType.I:
				case ElementType.Object:
				case ElementType.SzArray:
				case ElementType.MVar:
				{
					TypeDefinition typeDefinition = type.Resolve();
					if (typeDefinition == null || !typeDefinition.HasLayoutInfo)
					{
						break;
					}
					classSize = typeDefinition.ClassSize;
					break;
				}
				case ElementType.Ptr:
				case ElementType.FnPtr:
				{
					classSize = IntPtr.Size;
					break;
				}
				case ElementType.CModReqD:
				case ElementType.CModOpt:
				{
					return MetadataReader.GetFieldTypeSize(((IModifierType)type).ElementType);
				}
				default:
				{
					goto case ElementType.MVar;
				}
			}
			return classSize;
		}

		public MemoryStream GetManagedResourceStream(uint offset)
		{
			uint virtualAddress = this.image.Resources.VirtualAddress;
			Section sectionAtVirtualAddress = this.image.GetSectionAtVirtualAddress(virtualAddress);
			uint num = virtualAddress - sectionAtVirtualAddress.VirtualAddress + offset;
			byte[] data = sectionAtVirtualAddress.Data;
			int num1 = data[num] | data[num + 1] << 8 | data[num + 2] << 16 | data[num + 3] << 24;
			return new MemoryStream(data, (int)(num + 4), num1);
		}

		private static TMember GetMember<TMember>(Collection<TMember> members, MetadataToken token)
		where TMember : IMemberDefinition
		{
			for (int i = 0; i < members.Count; i++)
			{
				TMember item = members[i];
				if (item.MetadataToken == token)
				{
					return item;
				}
			}
			throw new ArgumentException();
		}

		private MemberReference GetMemberReference(uint rid)
		{
			this.InitializeMemberReferences();
			MemberReference memberReference = this.metadata.GetMemberReference(rid);
			if (memberReference != null)
			{
				return memberReference;
			}
			memberReference = this.ReadMemberReference(rid);
			if (memberReference != null && !memberReference.ContainsGenericParameter)
			{
				this.metadata.AddMemberReference(memberReference);
			}
			return memberReference;
		}

		public IEnumerable<MemberReference> GetMemberReferences()
		{
			this.InitializeMemberReferences();
			int tableLength = this.image.GetTableLength(Table.MemberRef);
			TypeSystem typeSystem = this.module.TypeSystem;
			MethodReference methodReference = new MethodReference(string.Empty, typeSystem.Void)
			{
				DeclaringType = new TypeReference(string.Empty, string.Empty, this.module, typeSystem.Corlib)
			};
			MemberReference[] memberReference = new MemberReference[tableLength];
			for (uint i = 1; (ulong)i <= (long)tableLength; i++)
			{
				this.context = methodReference;
				memberReference[i - 1] = this.GetMemberReference(i);
			}
			return memberReference;
		}

		public MethodDefinition GetMethodDefinition(uint rid)
		{
			this.InitializeTypeDefinitions();
			MethodDefinition methodDefinition = this.metadata.GetMethodDefinition(rid);
			if (methodDefinition != null)
			{
				return methodDefinition;
			}
			return this.LookupMethod(rid);
		}

		private MethodSpecification GetMethodSpecification(uint rid)
		{
			if (!this.MoveTo(Table.MethodSpec, rid))
			{
				return null;
			}
			MethodReference methodReference = (MethodReference)this.LookupToken(this.ReadMetadataToken(CodedIndex.MethodDefOrRef));
			MethodSpecification metadataToken = this.ReadMethodSpecSignature(this.ReadBlobIndex(), methodReference);
			metadataToken.token = new MetadataToken(Mono.Cecil.TokenType.MethodSpec, rid);
			return metadataToken;
		}

		private string GetModuleFileName(string name)
		{
			if (this.module.FullyQualifiedName == null)
			{
				throw new NotSupportedException();
			}
			return Path.Combine(Path.GetDirectoryName(this.module.FullyQualifiedName), name);
		}

		private ModuleReference GetModuleReferenceFromFile(MetadataToken token)
		{
			ModuleReference item;
			if (!this.MoveTo(Table.File, token.RID))
			{
				return null;
			}
			base.ReadUInt32();
			string str = this.ReadString();
			Collection<ModuleReference> moduleReferences = this.module.ModuleReferences;
			for (int i = 0; i < moduleReferences.Count; i++)
			{
				item = moduleReferences[i];
				if (item.Name == str)
				{
					return item;
				}
			}
			item = new ModuleReference(str);
			moduleReferences.Add(item);
			return item;
		}

		private TypeDefinition GetNestedTypeDeclaringType(TypeDefinition type)
		{
			uint num;
			if (!this.metadata.TryGetReverseNestedTypeMapping(type, out num))
			{
				return null;
			}
			this.metadata.RemoveReverseNestedTypeMapping(type);
			return this.GetTypeDefinition(num);
		}

		private static PropertyDefinition GetProperty(TypeDefinition type, MetadataToken token)
		{
			if (token.TokenType != Mono.Cecil.TokenType.Property)
			{
				throw new ArgumentException();
			}
			return MetadataReader.GetMember<PropertyDefinition>(type.Properties, token);
		}

		public TypeDefinition GetTypeDefinition(uint rid)
		{
			this.InitializeTypeDefinitions();
			TypeDefinition typeDefinition = this.metadata.GetTypeDefinition(rid);
			if (typeDefinition != null)
			{
				return typeDefinition;
			}
			return this.ReadTypeDefinition(rid);
		}

		public TypeReference GetTypeDefOrRef(MetadataToken token)
		{
			return (TypeReference)this.LookupToken(token);
		}

		public TypeReference GetTypeReference(string scope, string full_name)
		{
			this.InitializeTypeReferences();
			int length = (int)this.metadata.TypeReferences.Length;
			for (uint i = 1; (ulong)i <= (long)length; i++)
			{
				TypeReference typeReference = this.GetTypeReference(i);
				if (typeReference.FullName == full_name)
				{
					if (string.IsNullOrEmpty(scope))
					{
						return typeReference;
					}
					if (typeReference.Scope.Name == scope)
					{
						return typeReference;
					}
				}
			}
			return null;
		}

		private TypeReference GetTypeReference(uint rid)
		{
			this.InitializeTypeReferences();
			TypeReference typeReference = this.metadata.GetTypeReference(rid);
			if (typeReference != null)
			{
				return typeReference;
			}
			return this.ReadTypeReference(rid);
		}

		public IEnumerable<TypeReference> GetTypeReferences()
		{
			this.InitializeTypeReferences();
			int tableLength = this.image.GetTableLength(Table.TypeRef);
			TypeReference[] typeReference = new TypeReference[tableLength];
			for (uint i = 1; (ulong)i <= (long)tableLength; i++)
			{
				typeReference[i - 1] = this.GetTypeReference(i);
			}
			return typeReference;
		}

		private IMetadataScope GetTypeReferenceScope(MetadataToken scope)
		{
			IMetadataScope[] moduleReferences;
			if (scope.TokenType == Mono.Cecil.TokenType.Module)
			{
				return this.module;
			}
			Mono.Cecil.TokenType tokenType = scope.TokenType;
			if (tokenType == Mono.Cecil.TokenType.ModuleRef)
			{
				this.InitializeModuleReferences();
				moduleReferences = this.metadata.ModuleReferences;
			}
			else
			{
				if (tokenType != Mono.Cecil.TokenType.AssemblyRef)
				{
					throw new NotSupportedException();
				}
				this.InitializeAssemblyReferences();
				moduleReferences = this.metadata.AssemblyReferences;
			}
			uint rID = scope.RID - 1;
			if (rID < 0 || (ulong)rID >= (long)((int)moduleReferences.Length))
			{
				return null;
			}
			return moduleReferences[rID];
		}

		private TypeReference GetTypeSpecification(uint rid)
		{
			if (!this.MoveTo(Table.TypeSpec, rid))
			{
				return null;
			}
			TypeReference metadataToken = this.ReadSignature(this.ReadBlobIndex()).ReadTypeSignature();
			if (metadataToken.token.RID == 0)
			{
				metadataToken.token = new MetadataToken(Mono.Cecil.TokenType.TypeSpec, rid);
			}
			return metadataToken;
		}

		public bool HasCustomAttributes(ICustomAttributeProvider owner)
		{
			Range[] rangeArray;
			this.InitializeCustomAttributes();
			if (!this.metadata.TryGetCustomAttributeRanges(owner, out rangeArray))
			{
				return false;
			}
			return MetadataReader.RangesSize(rangeArray) > 0;
		}

		public bool HasEvents(TypeDefinition type)
		{
			Range range;
			this.InitializeEvents();
			if (!this.metadata.TryGetEventsRange(type, out range))
			{
				return false;
			}
			return range.Length != 0;
		}

		public bool HasFileResource()
		{
			int num = this.MoveTo(Table.File);
			if (num == 0)
			{
				return false;
			}
			for (uint i = 1; (ulong)i <= (long)num; i++)
			{
				if (this.ReadFileRecord(i).Col1 == 1)
				{
					return true;
				}
			}
			return false;
		}

		public bool HasGenericConstraints(GenericParameter generic_parameter)
		{
			MetadataToken[] metadataTokenArray;
			this.InitializeGenericConstraints();
			if (!this.metadata.TryGetGenericConstraintMapping(generic_parameter, out metadataTokenArray))
			{
				return false;
			}
			return metadataTokenArray.Length != 0;
		}

		public bool HasGenericParameters(IGenericParameterProvider provider)
		{
			Range[] rangeArray;
			this.InitializeGenericParameters();
			if (!this.metadata.TryGetGenericParameterRanges(provider, out rangeArray))
			{
				return false;
			}
			return MetadataReader.RangesSize(rangeArray) > 0;
		}

		public bool HasInterfaces(TypeDefinition type)
		{
			MetadataToken[] metadataTokenArray;
			this.InitializeInterfaces();
			return this.metadata.TryGetInterfaceMapping(type, out metadataTokenArray);
		}

		public bool HasMarshalInfo(IMarshalInfoProvider owner)
		{
			this.InitializeMarshalInfos();
			return this.metadata.FieldMarshals.ContainsKey(owner.MetadataToken);
		}

		public bool HasNestedTypes(TypeDefinition type)
		{
			uint[] numArray;
			this.InitializeNestedTypes();
			if (!this.metadata.TryGetNestedTypeMapping(type, out numArray))
			{
				return false;
			}
			return numArray.Length != 0;
		}

		public bool HasOverrides(MethodDefinition method)
		{
			MetadataToken[] metadataTokenArray;
			this.InitializeOverrides();
			if (!this.metadata.TryGetOverrideMapping(method, out metadataTokenArray))
			{
				return false;
			}
			return metadataTokenArray.Length != 0;
		}

		public bool HasProperties(TypeDefinition type)
		{
			Range range;
			this.InitializeProperties();
			if (!this.metadata.TryGetPropertiesRange(type, out range))
			{
				return false;
			}
			return range.Length != 0;
		}

		public bool HasSecurityDeclarations(ISecurityDeclarationProvider owner)
		{
			Range[] rangeArray;
			this.InitializeSecurityDeclarations();
			if (!this.metadata.TryGetSecurityDeclarationRanges(owner, out rangeArray))
			{
				return false;
			}
			return MetadataReader.RangesSize(rangeArray) > 0;
		}

		private void InitializeAssemblyReferences()
		{
			if (this.metadata.AssemblyReferences != null)
			{
				return;
			}
			int num = this.MoveTo(Table.AssemblyRef);
			MetadataSystem metadataSystem = this.metadata;
			AssemblyNameReference[] assemblyNameReferenceArray = new AssemblyNameReference[num];
			AssemblyNameReference[] assemblyNameReferenceArray1 = assemblyNameReferenceArray;
			metadataSystem.AssemblyReferences = assemblyNameReferenceArray;
			AssemblyNameReference[] assemblyNameReferenceArray2 = assemblyNameReferenceArray1;
			for (uint i = 0; (ulong)i < (long)num; i++)
			{
				AssemblyNameReference assemblyNameReference = new AssemblyNameReference()
				{
					token = new MetadataToken(Mono.Cecil.TokenType.AssemblyRef, i + 1)
				};
				this.PopulateVersionAndFlags(assemblyNameReference);
				byte[] numArray = this.ReadBlob();
				if (!assemblyNameReference.HasPublicKey)
				{
					assemblyNameReference.PublicKeyToken = numArray;
				}
				else
				{
					assemblyNameReference.PublicKey = numArray;
				}
				this.PopulateNameAndCulture(assemblyNameReference);
				assemblyNameReference.Hash = this.ReadBlob();
				assemblyNameReferenceArray2[i] = assemblyNameReference;
			}
		}

		private static void InitializeCollection(object o)
		{
		}

		private void InitializeConstants()
		{
			if (this.metadata.Constants != null)
			{
				return;
			}
			int num = this.MoveTo(Table.Constant);
			MetadataSystem metadataSystem = this.metadata;
			Dictionary<MetadataToken, Row<ElementType, uint>> metadataTokens = new Dictionary<MetadataToken, Row<ElementType, uint>>(num);
			Dictionary<MetadataToken, Row<ElementType, uint>> metadataTokens1 = metadataTokens;
			metadataSystem.Constants = metadataTokens;
			Dictionary<MetadataToken, Row<ElementType, uint>> metadataTokens2 = metadataTokens1;
			for (uint i = 1; (ulong)i <= (long)num; i++)
			{
				ElementType elementType = (ElementType)((byte)base.ReadUInt16());
				MetadataToken metadataToken = this.ReadMetadataToken(CodedIndex.HasConstant);
				metadataTokens2.Add(metadataToken, new Row<ElementType, uint>(elementType, this.ReadBlobIndex()));
			}
		}

		private void InitializeCustomAttributes()
		{
			if (this.metadata.CustomAttributes != null)
			{
				return;
			}
			this.metadata.CustomAttributes = this.InitializeRanges(Table.CustomAttribute, () => {
				MetadataToken metadataToken = this.ReadMetadataToken(CodedIndex.HasCustomAttribute);
				this.ReadMetadataToken(CodedIndex.CustomAttributeType);
				this.ReadBlobIndex();
				return metadataToken;
			});
		}

		private void InitializeEvents()
		{
			if (this.metadata.Events != null)
			{
				return;
			}
			int num = this.MoveTo(Table.EventMap);
			this.metadata.Events = new Dictionary<uint, Range>(num);
			for (uint i = 1; (ulong)i <= (long)num; i++)
			{
				uint num1 = this.ReadTableIndex(Table.TypeDef);
				Range range = this.ReadEventsRange(i);
				this.metadata.AddEventsRange(num1, range);
			}
		}

		private void InitializeFieldLayouts()
		{
			if (this.metadata.FieldLayouts != null)
			{
				return;
			}
			int num = this.MoveTo(Table.FieldLayout);
			MetadataSystem metadataSystem = this.metadata;
			Dictionary<uint, uint> nums = new Dictionary<uint, uint>(num);
			Dictionary<uint, uint> nums1 = nums;
			metadataSystem.FieldLayouts = nums;
			Dictionary<uint, uint> nums2 = nums1;
			for (int i = 0; i < num; i++)
			{
				uint num1 = base.ReadUInt32();
				nums2.Add(this.ReadTableIndex(Table.Field), num1);
			}
		}

		private void InitializeFieldRVAs()
		{
			if (this.metadata.FieldRVAs != null)
			{
				return;
			}
			int num = this.MoveTo(Table.FieldRVA);
			MetadataSystem metadataSystem = this.metadata;
			Dictionary<uint, uint> nums = new Dictionary<uint, uint>(num);
			Dictionary<uint, uint> nums1 = nums;
			metadataSystem.FieldRVAs = nums;
			Dictionary<uint, uint> nums2 = nums1;
			for (int i = 0; i < num; i++)
			{
				uint num1 = base.ReadUInt32();
				nums2.Add(this.ReadTableIndex(Table.Field), num1);
			}
		}

		private void InitializeFields()
		{
			if (this.metadata.Fields != null)
			{
				return;
			}
			this.metadata.Fields = new FieldDefinition[this.image.GetTableLength(Table.Field)];
		}

		private void InitializeGenericConstraints()
		{
			if (this.metadata.GenericConstraints != null)
			{
				return;
			}
			int num = this.MoveTo(Table.GenericParamConstraint);
			this.metadata.GenericConstraints = new Dictionary<uint, MetadataToken[]>(num);
			for (int i = 1; i <= num; i++)
			{
				this.AddGenericConstraintMapping(this.ReadTableIndex(Table.GenericParam), this.ReadMetadataToken(CodedIndex.TypeDefOrRef));
			}
		}

		private void InitializeGenericParameters()
		{
			if (this.metadata.GenericParameters != null)
			{
				return;
			}
			this.metadata.GenericParameters = this.InitializeRanges(Table.GenericParam, () => {
				base.Advance(4);
				MetadataToken metadataToken = this.ReadMetadataToken(CodedIndex.TypeOrMethodDef);
				this.ReadStringIndex();
				return metadataToken;
			});
		}

		private void InitializeInterfaces()
		{
			if (this.metadata.Interfaces != null)
			{
				return;
			}
			int num = this.MoveTo(Table.InterfaceImpl);
			this.metadata.Interfaces = new Dictionary<uint, MetadataToken[]>(num);
			for (int i = 0; i < num; i++)
			{
				uint num1 = this.ReadTableIndex(Table.TypeDef);
				this.AddInterfaceMapping(num1, this.ReadMetadataToken(CodedIndex.TypeDefOrRef));
			}
		}

		private void InitializeMarshalInfos()
		{
			if (this.metadata.FieldMarshals != null)
			{
				return;
			}
			int num = this.MoveTo(Table.FieldMarshal);
			MetadataSystem metadataSystem = this.metadata;
			Dictionary<MetadataToken, uint> metadataTokens = new Dictionary<MetadataToken, uint>(num);
			Dictionary<MetadataToken, uint> metadataTokens1 = metadataTokens;
			metadataSystem.FieldMarshals = metadataTokens;
			Dictionary<MetadataToken, uint> metadataTokens2 = metadataTokens1;
			for (int i = 0; i < num; i++)
			{
				MetadataToken metadataToken = this.ReadMetadataToken(CodedIndex.HasFieldMarshal);
				uint num1 = this.ReadBlobIndex();
				if (metadataToken.RID != 0)
				{
					metadataTokens2.Add(metadataToken, num1);
				}
			}
		}

		private void InitializeMemberReferences()
		{
			if (this.metadata.MemberReferences != null)
			{
				return;
			}
			this.metadata.MemberReferences = new MemberReference[this.image.GetTableLength(Table.MemberRef)];
		}

		private void InitializeMethods()
		{
			if (this.metadata.Methods != null)
			{
				return;
			}
			this.metadata.Methods = new MethodDefinition[this.image.GetTableLength(Table.Method)];
		}

		private void InitializeMethodSemantics()
		{
			if (this.metadata.Semantics != null)
			{
				return;
			}
			int num = this.MoveTo(Table.MethodSemantics);
			MetadataSystem metadataSystem = this.metadata;
			Dictionary<uint, Row<MethodSemanticsAttributes, MetadataToken>> nums = new Dictionary<uint, Row<MethodSemanticsAttributes, MetadataToken>>(0);
			Dictionary<uint, Row<MethodSemanticsAttributes, MetadataToken>> nums1 = nums;
			metadataSystem.Semantics = nums;
			Dictionary<uint, Row<MethodSemanticsAttributes, MetadataToken>> row = nums1;
			for (uint i = 0; (ulong)i < (long)num; i++)
			{
				MethodSemanticsAttributes methodSemanticsAttribute = (MethodSemanticsAttributes)base.ReadUInt16();
				uint num1 = this.ReadTableIndex(Table.Method);
				MetadataToken metadataToken = this.ReadMetadataToken(CodedIndex.HasSemantics);
				row[num1] = new Row<MethodSemanticsAttributes, MetadataToken>(methodSemanticsAttribute, metadataToken);
			}
		}

		private void InitializeModuleReferences()
		{
			if (this.metadata.ModuleReferences != null)
			{
				return;
			}
			int num = this.MoveTo(Table.ModuleRef);
			MetadataSystem metadataSystem = this.metadata;
			ModuleReference[] moduleReferenceArray = new ModuleReference[num];
			ModuleReference[] moduleReferenceArray1 = moduleReferenceArray;
			metadataSystem.ModuleReferences = moduleReferenceArray;
			ModuleReference[] moduleReferenceArray2 = moduleReferenceArray1;
			for (uint i = 0; (ulong)i < (long)num; i++)
			{
				ModuleReference moduleReference = new ModuleReference(this.ReadString())
				{
					token = new MetadataToken(Mono.Cecil.TokenType.ModuleRef, i + 1)
				};
				moduleReferenceArray2[i] = moduleReference;
			}
		}

		private void InitializeNestedTypes()
		{
			if (this.metadata.NestedTypes != null)
			{
				return;
			}
			int num = this.MoveTo(Table.NestedClass);
			this.metadata.NestedTypes = new Dictionary<uint, uint[]>(num);
			this.metadata.ReverseNestedTypes = new Dictionary<uint, uint>(num);
			if (num == 0)
			{
				return;
			}
			for (int i = 1; i <= num; i++)
			{
				uint num1 = this.ReadTableIndex(Table.TypeDef);
				this.AddNestedMapping(this.ReadTableIndex(Table.TypeDef), num1);
			}
		}

		private void InitializeOverrides()
		{
			if (this.metadata.Overrides != null)
			{
				return;
			}
			int num = this.MoveTo(Table.MethodImpl);
			this.metadata.Overrides = new Dictionary<uint, MetadataToken[]>(num);
			for (int i = 1; i <= num; i++)
			{
				this.ReadTableIndex(Table.TypeDef);
				MetadataToken metadataToken = this.ReadMetadataToken(CodedIndex.MethodDefOrRef);
				if (metadataToken.TokenType != Mono.Cecil.TokenType.Method)
				{
					throw new NotSupportedException();
				}
				MetadataToken metadataToken1 = this.ReadMetadataToken(CodedIndex.MethodDefOrRef);
				this.AddOverrideMapping(metadataToken.RID, metadataToken1);
			}
		}

		private void InitializePInvokes()
		{
			if (this.metadata.PInvokes != null)
			{
				return;
			}
			int num = this.MoveTo(Table.ImplMap);
			MetadataSystem metadataSystem = this.metadata;
			Dictionary<uint, Row<PInvokeAttributes, uint, uint>> nums = new Dictionary<uint, Row<PInvokeAttributes, uint, uint>>(num);
			Dictionary<uint, Row<PInvokeAttributes, uint, uint>> nums1 = nums;
			metadataSystem.PInvokes = nums;
			Dictionary<uint, Row<PInvokeAttributes, uint, uint>> nums2 = nums1;
			for (int i = 1; i <= num; i++)
			{
				PInvokeAttributes pInvokeAttribute = (PInvokeAttributes)base.ReadUInt16();
				MetadataToken metadataToken = this.ReadMetadataToken(CodedIndex.MemberForwarded);
				uint num1 = this.ReadStringIndex();
				uint num2 = this.ReadTableIndex(Table.File);
				if (metadataToken.TokenType == Mono.Cecil.TokenType.Method)
				{
					nums2.Add(metadataToken.RID, new Row<PInvokeAttributes, uint, uint>(pInvokeAttribute, num1, num2));
				}
			}
		}

		private void InitializeProperties()
		{
			if (this.metadata.Properties != null)
			{
				return;
			}
			int num = this.MoveTo(Table.PropertyMap);
			this.metadata.Properties = new Dictionary<uint, Range>(num);
			for (uint i = 1; (ulong)i <= (long)num; i++)
			{
				uint num1 = this.ReadTableIndex(Table.TypeDef);
				Range range = this.ReadPropertiesRange(i);
				this.metadata.AddPropertiesRange(num1, range);
			}
		}

		private Dictionary<MetadataToken, Range[]> InitializeRanges(Table table, Func<MetadataToken> get_next)
		{
			int num = this.MoveTo(table);
			Dictionary<MetadataToken, Range[]> metadataTokens = new Dictionary<MetadataToken, Range[]>(num);
			if (num == 0)
			{
				return metadataTokens;
			}
			MetadataToken zero = MetadataToken.Zero;
			Range range = new Range(1, 0);
			for (uint i = 1; (ulong)i <= (long)num; i++)
			{
				MetadataToken getNext = get_next();
				if (i == 1)
				{
					zero = getNext;
					range.Length++;
				}
				else if (getNext == zero)
				{
					range.Length++;
				}
				else
				{
					MetadataReader.AddRange(metadataTokens, zero, range);
					range = new Range(i, 1);
					zero = getNext;
				}
			}
			MetadataReader.AddRange(metadataTokens, zero, range);
			return metadataTokens;
		}

		private void InitializeSecurityDeclarations()
		{
			if (this.metadata.SecurityDeclarations != null)
			{
				return;
			}
			this.metadata.SecurityDeclarations = this.InitializeRanges(Table.DeclSecurity, () => {
				base.ReadUInt16();
				MetadataToken metadataToken = this.ReadMetadataToken(CodedIndex.HasDeclSecurity);
				this.ReadBlobIndex();
				return metadataToken;
			});
		}

		private void InitializeTypeDefinitions()
		{
			if (this.metadata.Types != null)
			{
				return;
			}
			this.InitializeNestedTypes();
			this.InitializeFields();
			this.InitializeMethods();
			int num = this.MoveTo(Table.TypeDef);
			MetadataSystem metadataSystem = this.metadata;
			TypeDefinition[] typeDefinitionArray = new TypeDefinition[num];
			TypeDefinition[] typeDefinitionArray1 = typeDefinitionArray;
			metadataSystem.Types = typeDefinitionArray;
			TypeDefinition[] typeDefinitionArray2 = typeDefinitionArray1;
			for (uint i = 0; (ulong)i < (long)num; i++)
			{
				if (typeDefinitionArray2[i] == null)
				{
					typeDefinitionArray2[i] = this.ReadType(i + 1);
				}
			}
		}

		private void InitializeTypeLayouts()
		{
			if (this.metadata.ClassLayouts != null)
			{
				return;
			}
			int num = this.MoveTo(Table.ClassLayout);
			MetadataSystem metadataSystem = this.metadata;
			Dictionary<uint, Row<ushort, uint>> nums = new Dictionary<uint, Row<ushort, uint>>(num);
			Dictionary<uint, Row<ushort, uint>> nums1 = nums;
			metadataSystem.ClassLayouts = nums;
			Dictionary<uint, Row<ushort, uint>> nums2 = nums1;
			for (uint i = 0; (ulong)i < (long)num; i++)
			{
				ushort num1 = base.ReadUInt16();
				uint num2 = base.ReadUInt32();
				uint num3 = this.ReadTableIndex(Table.TypeDef);
				nums2.Add(num3, new Row<ushort, uint>(num1, num2));
			}
		}

		private void InitializeTypeReferences()
		{
			if (this.metadata.TypeReferences != null)
			{
				return;
			}
			this.metadata.TypeReferences = new TypeReference[this.image.GetTableLength(Table.TypeRef)];
		}

		private static bool IsDeleted(IMemberDefinition member)
		{
			if (!member.IsSpecialName)
			{
				return false;
			}
			return member.Name == "_Deleted";
		}

		private static bool IsNested(TypeAttributes attributes)
		{
			switch (attributes & TypeAttributes.VisibilityMask)
			{
				case TypeAttributes.NestedPublic:
				case TypeAttributes.NestedPrivate:
				case TypeAttributes.NestedFamily:
				case TypeAttributes.NestedAssembly:
				case TypeAttributes.NestedFamANDAssem:
				case TypeAttributes.VisibilityMask:
				{
					return true;
				}
			}
			return false;
		}

		private FieldDefinition LookupField(uint rid)
		{
			TypeDefinition fieldDeclaringType = this.metadata.GetFieldDeclaringType(rid);
			if (fieldDeclaringType == null)
			{
				return null;
			}
			MetadataReader.InitializeCollection(fieldDeclaringType.Fields);
			return this.metadata.GetFieldDefinition(rid);
		}

		private MethodDefinition LookupMethod(uint rid)
		{
			TypeDefinition methodDeclaringType = this.metadata.GetMethodDeclaringType(rid);
			if (methodDeclaringType == null)
			{
				return null;
			}
			MetadataReader.InitializeCollection(methodDeclaringType.Methods);
			return this.metadata.GetMethodDefinition(rid);
		}

		public IMetadataTokenProvider LookupToken(MetadataToken token)
		{
			IMetadataTokenProvider typeReference;
			uint rID = token.RID;
			if (rID == 0)
			{
				return null;
			}
			int num = this.position;
			IGenericContext genericContext = this.context;
			Mono.Cecil.TokenType tokenType = token.TokenType;
			if (tokenType <= Mono.Cecil.TokenType.Field)
			{
				if (tokenType == Mono.Cecil.TokenType.TypeRef)
				{
					typeReference = this.GetTypeReference(rID);
				}
				else if (tokenType == Mono.Cecil.TokenType.TypeDef)
				{
					typeReference = this.GetTypeDefinition(rID);
				}
				else
				{
					if (tokenType != Mono.Cecil.TokenType.Field)
					{
						return null;
					}
					typeReference = this.GetFieldDefinition(rID);
				}
			}
			else if (tokenType <= Mono.Cecil.TokenType.MemberRef)
			{
				if (tokenType == Mono.Cecil.TokenType.Method)
				{
					typeReference = this.GetMethodDefinition(rID);
				}
				else
				{
					if (tokenType != Mono.Cecil.TokenType.MemberRef)
					{
						return null;
					}
					typeReference = this.GetMemberReference(rID);
				}
			}
			else if (tokenType == Mono.Cecil.TokenType.TypeSpec)
			{
				typeReference = this.GetTypeSpecification(rID);
			}
			else
			{
				if (tokenType != Mono.Cecil.TokenType.MethodSpec)
				{
					return null;
				}
				typeReference = this.GetMethodSpecification(rID);
			}
			this.position = num;
			this.context = genericContext;
			return typeReference;
		}

		private int MoveTo(Table table)
		{
			TableInformation item = this.image.TableHeap[table];
			if (item.Length != 0)
			{
				this.Position = item.Offset;
			}
			return (int)item.Length;
		}

		private bool MoveTo(Table table, uint row)
		{
			TableInformation item = this.image.TableHeap[table];
			uint length = item.Length;
			if (length == 0 || row > length)
			{
				return false;
			}
			this.Position = item.Offset + item.RowSize * (row - 1);
			return true;
		}

		public ModuleDefinition Populate(ModuleDefinition module)
		{
			if (this.MoveTo(Table.Module) == 0)
			{
				return module;
			}
			base.Advance(2);
			module.Name = this.ReadString();
			module.Mvid = this.image.GuidHeap.Read(this.ReadByIndexSize(this.image.GuidHeap.IndexSize));
			return module;
		}

		private void PopulateNameAndCulture(AssemblyNameReference name)
		{
			name.Name = this.ReadString();
			name.Culture = this.ReadString();
		}

		private void PopulateVersionAndFlags(AssemblyNameReference name)
		{
			name.Version = new Version((int)base.ReadUInt16(), (int)base.ReadUInt16(), (int)base.ReadUInt16(), (int)base.ReadUInt16());
			name.Attributes = (AssemblyAttributes)base.ReadUInt32();
		}

		private static int RangesSize(Range[] ranges)
		{
			uint length = 0;
			for (int i = 0; i < (int)ranges.Length; i++)
			{
				length += ranges[i].Length;
			}
			return (int)length;
		}

		public MethodSemanticsAttributes ReadAllSemantics(MethodDefinition method)
		{
			this.ReadAllSemantics(method.DeclaringType);
			return method.SemanticsAttributes;
		}

		private void ReadAllSemantics(TypeDefinition type)
		{
			Collection<MethodDefinition> methods = type.Methods;
			for (int i = 0; i < methods.Count; i++)
			{
				MethodDefinition item = methods[i];
				if (!item.sem_attrs_ready)
				{
					item.sem_attrs = this.ReadMethodSemantics(item);
					item.sem_attrs_ready = true;
				}
			}
		}

		public AssemblyNameDefinition ReadAssemblyNameDefinition()
		{
			if (this.MoveTo(Table.Assembly) == 0)
			{
				return null;
			}
			AssemblyNameDefinition assemblyNameDefinition = new AssemblyNameDefinition()
			{
				HashAlgorithm = (AssemblyHashAlgorithm)base.ReadUInt32()
			};
			this.PopulateVersionAndFlags(assemblyNameDefinition);
			assemblyNameDefinition.PublicKey = this.ReadBlob();
			this.PopulateNameAndCulture(assemblyNameDefinition);
			return assemblyNameDefinition;
		}

		public Collection<AssemblyNameReference> ReadAssemblyReferences()
		{
			this.InitializeAssemblyReferences();
			return new Collection<AssemblyNameReference>(this.metadata.AssemblyReferences);
		}

		private byte[] ReadBlob()
		{
			BlobHeap blobHeap = this.image.BlobHeap;
			if (blobHeap != null)
			{
				return blobHeap.Read(this.ReadBlobIndex());
			}
			this.position += 2;
			return Empty<byte>.Array;
		}

		private byte[] ReadBlob(uint signature)
		{
			BlobHeap blobHeap = this.image.BlobHeap;
			if (blobHeap == null)
			{
				return Empty<byte>.Array;
			}
			return blobHeap.Read(signature);
		}

		private uint ReadBlobIndex()
		{
			BlobHeap blobHeap = this.image.BlobHeap;
			return this.ReadByIndexSize((blobHeap != null ? blobHeap.IndexSize : 2));
		}

		private uint ReadByIndexSize(int size)
		{
			if (size == 4)
			{
				return base.ReadUInt32();
			}
			return base.ReadUInt16();
		}

		public Mono.Cecil.CallSite ReadCallSite(MetadataToken token)
		{
			if (!this.MoveTo(Table.StandAloneSig, token.RID))
			{
				return null;
			}
			uint num = this.ReadBlobIndex();
			Mono.Cecil.CallSite callSite = new Mono.Cecil.CallSite();
			this.ReadMethodSignature(num, callSite);
			callSite.MetadataToken = token;
			return callSite;
		}

		public object ReadConstant(IConstantProvider owner)
		{
			Row<ElementType, uint> row;
			this.InitializeConstants();
			if (!this.metadata.Constants.TryGetValue(owner.MetadataToken, out row))
			{
				return Mixin.NoValue;
			}
			this.metadata.Constants.Remove(owner.MetadataToken);
			ElementType col1 = row.Col1;
			if (col1 == ElementType.String)
			{
				return MetadataReader.ReadConstantString(this.ReadBlob(row.Col2));
			}
			if (col1 == ElementType.Class || col1 == ElementType.Object)
			{
				return null;
			}
			return this.ReadConstantPrimitive((ElementType)row.Col1, row.Col2);
		}

		private object ReadConstantPrimitive(ElementType type, uint signature)
		{
			return this.ReadSignature(signature).ReadConstantSignature(type);
		}

		private static string ReadConstantString(byte[] blob)
		{
			int length = (int)blob.Length;
			if ((length & 1) == 1)
			{
				length--;
			}
			return Encoding.Unicode.GetString(blob, 0, length);
		}

		public byte[] ReadCustomAttributeBlob(uint signature)
		{
			return this.ReadBlob(signature);
		}

		private void ReadCustomAttributeRange(Range range, Collection<CustomAttribute> custom_attributes)
		{
			if (!this.MoveTo(Table.CustomAttribute, range.Start))
			{
				return;
			}
			for (int i = 0; (long)i < (ulong)range.Length; i++)
			{
				this.ReadMetadataToken(CodedIndex.HasCustomAttribute);
				MethodReference methodReference = (MethodReference)this.LookupToken(this.ReadMetadataToken(CodedIndex.CustomAttributeType));
				custom_attributes.Add(new CustomAttribute(this.ReadBlobIndex(), methodReference));
			}
		}

		public Collection<CustomAttribute> ReadCustomAttributes(ICustomAttributeProvider owner)
		{
			Range[] rangeArray;
			this.InitializeCustomAttributes();
			if (!this.metadata.TryGetCustomAttributeRanges(owner, out rangeArray))
			{
				return new Collection<CustomAttribute>();
			}
			Collection<CustomAttribute> customAttributes = new Collection<CustomAttribute>(MetadataReader.RangesSize(rangeArray));
			for (int i = 0; i < (int)rangeArray.Length; i++)
			{
				this.ReadCustomAttributeRange(rangeArray[i], customAttributes);
			}
			this.metadata.RemoveCustomAttributeRange(owner);
			return customAttributes;
		}

		public void ReadCustomAttributeSignature(CustomAttribute attribute)
		{
			SignatureReader signatureReader = this.ReadSignature(attribute.signature);
			if (!signatureReader.CanReadMore())
			{
				return;
			}
			if (signatureReader.ReadUInt16() != 1)
			{
				throw new InvalidOperationException();
			}
			MethodReference constructor = attribute.Constructor;
			if (constructor.HasParameters)
			{
				signatureReader.ReadCustomAttributeConstructorArguments(attribute, constructor.Parameters);
			}
			if (!signatureReader.CanReadMore())
			{
				return;
			}
			ushort num = signatureReader.ReadUInt16();
			if (num == 0)
			{
				return;
			}
			signatureReader.ReadCustomAttributeNamedArguments(num, ref attribute.fields, ref attribute.properties);
		}

		public MethodDefinition ReadEntryPoint()
		{
			if (this.module.Image.EntryPointToken == 0)
			{
				return null;
			}
			MetadataToken metadataToken = new MetadataToken(this.module.Image.EntryPointToken);
			return this.GetMethodDefinition(metadataToken.RID);
		}

		private void ReadEvent(uint event_rid, Collection<EventDefinition> events)
		{
			EventAttributes eventAttribute = (EventAttributes)base.ReadUInt16();
			string str = this.ReadString();
			TypeReference typeDefOrRef = this.GetTypeDefOrRef(this.ReadMetadataToken(CodedIndex.TypeDefOrRef));
			EventDefinition eventDefinition = new EventDefinition(str, eventAttribute, typeDefOrRef)
			{
				token = new MetadataToken(Mono.Cecil.TokenType.Event, event_rid)
			};
			if (MetadataReader.IsDeleted(eventDefinition))
			{
				return;
			}
			events.Add(eventDefinition);
		}

		public Collection<EventDefinition> ReadEvents(TypeDefinition type)
		{
			Range range;
			this.InitializeEvents();
			if (!this.metadata.TryGetEventsRange(type, out range))
			{
				return new MemberDefinitionCollection<EventDefinition>(type);
			}
			MemberDefinitionCollection<EventDefinition> memberDefinitionCollection = new MemberDefinitionCollection<EventDefinition>(type, (int)range.Length);
			this.metadata.RemoveEventsRange(type);
			if (range.Length == 0)
			{
				return memberDefinitionCollection;
			}
			this.context = type;
			if (this.MoveTo(Table.EventPtr, range.Start))
			{
				this.ReadPointers<EventDefinition>(Table.EventPtr, Table.Event, range, memberDefinitionCollection, new Action<uint, Collection<EventDefinition>>(this.ReadEvent));
			}
			else
			{
				if (!this.MoveTo(Table.Event, range.Start))
				{
					return memberDefinitionCollection;
				}
				for (uint i = 0; i < range.Length; i++)
				{
					this.ReadEvent(range.Start + i, memberDefinitionCollection);
				}
			}
			return memberDefinitionCollection;
		}

		private Range ReadEventsRange(uint rid)
		{
			return this.ReadListRange(rid, Table.EventMap, Table.Event);
		}

		public Collection<ExportedType> ReadExportedTypes()
		{
			int num = this.MoveTo(Table.ExportedType);
			if (num == 0)
			{
				return new Collection<ExportedType>();
			}
			Collection<ExportedType> exportedTypes = new Collection<ExportedType>(num);
			for (int i = 1; i <= num; i++)
			{
				TypeAttributes typeAttribute = (TypeAttributes)base.ReadUInt32();
				uint num1 = base.ReadUInt32();
				string str = this.ReadString();
				string str1 = this.ReadString();
				MetadataToken metadataToken = this.ReadMetadataToken(CodedIndex.Implementation);
				ExportedType item = null;
				IMetadataScope exportedTypeScope = null;
				Mono.Cecil.TokenType tokenType = metadataToken.TokenType;
				if (tokenType == Mono.Cecil.TokenType.AssemblyRef || tokenType == Mono.Cecil.TokenType.File)
				{
					exportedTypeScope = this.GetExportedTypeScope(metadataToken);
				}
				else if (tokenType == Mono.Cecil.TokenType.ExportedType)
				{
					item = exportedTypes[metadataToken.RID - 1];
				}
				ExportedType exportedType = new ExportedType(str1, str, this.module, exportedTypeScope)
				{
					Attributes = typeAttribute,
					Identifier = (int)num1,
					DeclaringType = item,
					token = new MetadataToken(Mono.Cecil.TokenType.ExportedType, i)
				};
				exportedTypes.Add(exportedType);
			}
			return exportedTypes;
		}

		private void ReadField(uint field_rid, Collection<FieldDefinition> fields)
		{
			FieldAttributes fieldAttribute = (FieldAttributes)base.ReadUInt16();
			string str = this.ReadString();
			uint num = this.ReadBlobIndex();
			FieldDefinition fieldDefinition = new FieldDefinition(str, fieldAttribute, this.ReadFieldType(num))
			{
				token = new MetadataToken(Mono.Cecil.TokenType.Field, field_rid)
			};
			this.metadata.AddFieldDefinition(fieldDefinition);
			if (MetadataReader.IsDeleted(fieldDefinition))
			{
				return;
			}
			fields.Add(fieldDefinition);
		}

		public int ReadFieldLayout(FieldDefinition field)
		{
			uint num;
			this.InitializeFieldLayouts();
			uint rID = field.token.RID;
			if (!this.metadata.FieldLayouts.TryGetValue(rID, out num))
			{
				return -1;
			}
			this.metadata.FieldLayouts.Remove(rID);
			return (int)num;
		}

		public int ReadFieldRVA(FieldDefinition field)
		{
			uint num;
			this.InitializeFieldRVAs();
			uint rID = field.token.RID;
			if (!this.metadata.FieldRVAs.TryGetValue(rID, out num))
			{
				return 0;
			}
			int fieldTypeSize = MetadataReader.GetFieldTypeSize(field.FieldType);
			if (fieldTypeSize == 0 || num == 0)
			{
				return 0;
			}
			this.metadata.FieldRVAs.Remove(rID);
			field.InitialValue = this.GetFieldInitializeValue(fieldTypeSize, num);
			return (int)num;
		}

		public Collection<FieldDefinition> ReadFields(TypeDefinition type)
		{
			Range fieldsRange = type.fields_range;
			if (fieldsRange.Length == 0)
			{
				return new MemberDefinitionCollection<FieldDefinition>(type);
			}
			MemberDefinitionCollection<FieldDefinition> memberDefinitionCollection = new MemberDefinitionCollection<FieldDefinition>(type, (int)fieldsRange.Length);
			this.context = type;
			if (this.MoveTo(Table.FieldPtr, fieldsRange.Start))
			{
				this.ReadPointers<FieldDefinition>(Table.FieldPtr, Table.Field, fieldsRange, memberDefinitionCollection, new Action<uint, Collection<FieldDefinition>>(this.ReadField));
			}
			else
			{
				if (!this.MoveTo(Table.Field, fieldsRange.Start))
				{
					return memberDefinitionCollection;
				}
				for (uint i = 0; i < fieldsRange.Length; i++)
				{
					this.ReadField(fieldsRange.Start + i, memberDefinitionCollection);
				}
			}
			return memberDefinitionCollection;
		}

		private Range ReadFieldsRange(uint type_index)
		{
			return this.ReadListRange(type_index, Table.TypeDef, Table.Field);
		}

		private TypeReference ReadFieldType(uint signature)
		{
			SignatureReader signatureReader = this.ReadSignature(signature);
			if (signatureReader.ReadByte() != 6)
			{
				throw new NotSupportedException();
			}
			return signatureReader.ReadTypeSignature();
		}

		private Row<Mono.Cecil.FileAttributes, string, uint> ReadFileRecord(uint rid)
		{
			int num = this.position;
			if (!this.MoveTo(Table.File, rid))
			{
				throw new ArgumentException();
			}
			this.position = num;
			return new Row<Mono.Cecil.FileAttributes, string, uint>((Mono.Cecil.FileAttributes)base.ReadUInt32(), this.ReadString(), this.ReadBlobIndex());
		}

		public Collection<TypeReference> ReadGenericConstraints(GenericParameter generic_parameter)
		{
			MetadataToken[] metadataTokenArray;
			this.InitializeGenericConstraints();
			if (!this.metadata.TryGetGenericConstraintMapping(generic_parameter, out metadataTokenArray))
			{
				return new Collection<TypeReference>();
			}
			Collection<TypeReference> typeReferences = new Collection<TypeReference>((int)metadataTokenArray.Length);
			this.context = (IGenericContext)generic_parameter.Owner;
			for (int i = 0; i < (int)metadataTokenArray.Length; i++)
			{
				typeReferences.Add(this.GetTypeDefOrRef(metadataTokenArray[i]));
			}
			this.metadata.RemoveGenericConstraintMapping(generic_parameter);
			return typeReferences;
		}

		public Collection<GenericParameter> ReadGenericParameters(IGenericParameterProvider provider)
		{
			Range[] rangeArray;
			this.InitializeGenericParameters();
			if (!this.metadata.TryGetGenericParameterRanges(provider, out rangeArray))
			{
				return new GenericParameterCollection(provider);
			}
			this.metadata.RemoveGenericParameterRange(provider);
			GenericParameterCollection genericParameterCollection = new GenericParameterCollection(provider, MetadataReader.RangesSize(rangeArray));
			for (int i = 0; i < (int)rangeArray.Length; i++)
			{
				this.ReadGenericParametersRange(rangeArray[i], provider, genericParameterCollection);
			}
			return genericParameterCollection;
		}

		private void ReadGenericParametersRange(Range range, IGenericParameterProvider provider, GenericParameterCollection generic_parameters)
		{
			if (!this.MoveTo(Table.GenericParam, range.Start))
			{
				return;
			}
			for (uint i = 0; i < range.Length; i++)
			{
				base.ReadUInt16();
				GenericParameterAttributes genericParameterAttribute = (GenericParameterAttributes)base.ReadUInt16();
				this.ReadMetadataToken(CodedIndex.TypeOrMethodDef);
				GenericParameter genericParameter = new GenericParameter(this.ReadString(), provider)
				{
					token = new MetadataToken(Mono.Cecil.TokenType.GenericParam, range.Start + i),
					Attributes = genericParameterAttribute
				};
				generic_parameters.Add(genericParameter);
			}
		}

		public Collection<TypeReference> ReadInterfaces(TypeDefinition type)
		{
			MetadataToken[] metadataTokenArray;
			this.InitializeInterfaces();
			if (!this.metadata.TryGetInterfaceMapping(type, out metadataTokenArray))
			{
				return new Collection<TypeReference>();
			}
			Collection<TypeReference> typeReferences = new Collection<TypeReference>((int)metadataTokenArray.Length);
			this.context = type;
			for (int i = 0; i < (int)metadataTokenArray.Length; i++)
			{
				typeReferences.Add(this.GetTypeDefOrRef(metadataTokenArray[i]));
			}
			this.metadata.RemoveInterfaceMapping(type);
			return typeReferences;
		}

		private Range ReadListRange(uint current_index, Table current, Table target)
		{
			uint length;
			Range start = new Range()
			{
				Start = this.ReadTableIndex(target)
			};
			TableInformation item = this.image.TableHeap[current];
			if (current_index != item.Length)
			{
				uint position = this.Position;
				this.Position = this.Position + (uint)((ulong)item.RowSize - (long)this.image.GetTableIndexSize(target));
				length = this.ReadTableIndex(target);
				this.Position = position;
			}
			else
			{
				length = this.image.TableHeap[target].Length + 1;
			}
			start.Length = length - start.Start;
			return start;
		}

		public MarshalInfo ReadMarshalInfo(IMarshalInfoProvider owner)
		{
			uint num;
			this.InitializeMarshalInfos();
			if (!this.metadata.FieldMarshals.TryGetValue(owner.MetadataToken, out num))
			{
				return null;
			}
			SignatureReader signatureReader = this.ReadSignature(num);
			this.metadata.FieldMarshals.Remove(owner.MetadataToken);
			return signatureReader.ReadMarshalInfo();
		}

		private MemberReference ReadMemberReference(uint rid)
		{
			MemberReference metadataToken;
			if (!this.MoveTo(Table.MemberRef, rid))
			{
				return null;
			}
			MetadataToken metadataToken1 = this.ReadMetadataToken(CodedIndex.MemberRefParent);
			string str = this.ReadString();
			uint num = this.ReadBlobIndex();
			Mono.Cecil.TokenType tokenType = metadataToken1.TokenType;
			if (tokenType > Mono.Cecil.TokenType.TypeDef)
			{
				if (tokenType != Mono.Cecil.TokenType.Method)
				{
					goto Label2;
				}
				metadataToken = this.ReadMethodMemberReference(metadataToken1, str, num);
				metadataToken.token = new MetadataToken(Mono.Cecil.TokenType.MemberRef, rid);
				return metadataToken;
			}
			else
			{
				if (tokenType == Mono.Cecil.TokenType.TypeRef || tokenType == Mono.Cecil.TokenType.TypeDef)
				{
					metadataToken = this.ReadTypeMemberReference(metadataToken1, str, num);
					metadataToken.token = new MetadataToken(Mono.Cecil.TokenType.MemberRef, rid);
					return metadataToken;
				}
				throw new NotSupportedException();
			}
			metadataToken = this.ReadTypeMemberReference(metadataToken1, str, num);
			metadataToken.token = new MetadataToken(Mono.Cecil.TokenType.MemberRef, rid);
			return metadataToken;
		Label2:
			if (tokenType != Mono.Cecil.TokenType.TypeSpec)
			{
				throw new NotSupportedException();
			}
			else
			{
				metadataToken = this.ReadTypeMemberReference(metadataToken1, str, num);
				metadataToken.token = new MetadataToken(Mono.Cecil.TokenType.MemberRef, rid);
				return metadataToken;
			}
		}

		private MemberReference ReadMemberReferenceSignature(uint signature, TypeReference declaring_type)
		{
			SignatureReader signatureReader = this.ReadSignature(signature);
			if (signatureReader.buffer[signatureReader.position] != 6)
			{
				MethodReference methodReference = new MethodReference()
				{
					DeclaringType = declaring_type
				};
				signatureReader.ReadMethodSignature(methodReference);
				return methodReference;
			}
			signatureReader.position++;
			return new FieldReference()
			{
				DeclaringType = declaring_type,
				FieldType = signatureReader.ReadTypeSignature()
			};
		}

		private MetadataToken ReadMetadataToken(CodedIndex index)
		{
			return index.GetMetadataToken(this.ReadByIndexSize(this.GetCodedIndexSize(index)));
		}

		private void ReadMethod(uint method_rid, Collection<MethodDefinition> methods)
		{
			MethodDefinition methodDefinition = new MethodDefinition()
			{
				rva = base.ReadUInt32(),
				ImplAttributes = (MethodImplAttributes)base.ReadUInt16(),
				Attributes = (MethodAttributes)base.ReadUInt16(),
				Name = this.ReadString(),
				token = new MetadataToken(Mono.Cecil.TokenType.Method, method_rid)
			};
			if (MetadataReader.IsDeleted(methodDefinition))
			{
				return;
			}
			methods.Add(methodDefinition);
			uint num = this.ReadBlobIndex();
			Range range = this.ReadParametersRange(method_rid);
			this.context = methodDefinition;
			this.ReadMethodSignature(num, methodDefinition);
			this.metadata.AddMethodDefinition(methodDefinition);
			if (range.Length == 0)
			{
				return;
			}
			int num1 = this.position;
			this.ReadParameters(methodDefinition, range);
			this.position = num1;
		}

		public MethodBody ReadMethodBody(MethodDefinition method)
		{
			return this.code.ReadMethodBody(method);
		}

		private MemberReference ReadMethodMemberReference(MetadataToken token, string name, uint signature)
		{
			MethodDefinition methodDefinition = this.GetMethodDefinition(token.RID);
			this.context = methodDefinition;
			MemberReference memberReference = this.ReadMemberReferenceSignature(signature, methodDefinition.DeclaringType);
			memberReference.Name = name;
			return memberReference;
		}

		public PropertyDefinition ReadMethods(PropertyDefinition property)
		{
			this.ReadAllSemantics(property.DeclaringType);
			return property;
		}

		public EventDefinition ReadMethods(EventDefinition @event)
		{
			this.ReadAllSemantics(@event.DeclaringType);
			return @event;
		}

		public Collection<MethodDefinition> ReadMethods(TypeDefinition type)
		{
			Range methodsRange = type.methods_range;
			if (methodsRange.Length == 0)
			{
				return new MemberDefinitionCollection<MethodDefinition>(type);
			}
			MemberDefinitionCollection<MethodDefinition> memberDefinitionCollection = new MemberDefinitionCollection<MethodDefinition>(type, (int)methodsRange.Length);
			if (this.MoveTo(Table.MethodPtr, methodsRange.Start))
			{
				this.ReadPointers<MethodDefinition>(Table.MethodPtr, Table.Method, methodsRange, memberDefinitionCollection, new Action<uint, Collection<MethodDefinition>>(this.ReadMethod));
			}
			else
			{
				if (!this.MoveTo(Table.Method, methodsRange.Start))
				{
					return memberDefinitionCollection;
				}
				for (uint i = 0; i < methodsRange.Length; i++)
				{
					this.ReadMethod(methodsRange.Start + i, memberDefinitionCollection);
				}
			}
			return memberDefinitionCollection;
		}

		private MethodSemanticsAttributes ReadMethodSemantics(MethodDefinition method)
		{
			Row<MethodSemanticsAttributes, MetadataToken> row;
			this.InitializeMethodSemantics();
			if (!this.metadata.Semantics.TryGetValue(method.token.RID, out row))
			{
				return MethodSemanticsAttributes.None;
			}
			TypeDefinition declaringType = method.DeclaringType;
			MethodSemanticsAttributes col1 = row.Col1;
			if (col1 <= MethodSemanticsAttributes.AddOn)
			{
				switch (col1)
				{
					case MethodSemanticsAttributes.Setter:
					{
						MetadataReader.GetProperty(declaringType, row.Col2).set_method = method;
						break;
					}
					case MethodSemanticsAttributes.Getter:
					{
						MetadataReader.GetProperty(declaringType, row.Col2).get_method = method;
						break;
					}
					case MethodSemanticsAttributes.Setter | MethodSemanticsAttributes.Getter:
					{
						throw new NotSupportedException();
					}
					case MethodSemanticsAttributes.Other:
					{
						Mono.Cecil.TokenType tokenType = row.Col2.TokenType;
						if (tokenType == Mono.Cecil.TokenType.Event)
						{
							EventDefinition @event = MetadataReader.GetEvent(declaringType, row.Col2);
							if (@event.other_methods == null)
							{
								@event.other_methods = new Collection<MethodDefinition>();
							}
							@event.other_methods.Add(method);
							break;
						}
						else
						{
							if (tokenType != Mono.Cecil.TokenType.Property)
							{
								throw new NotSupportedException();
							}
							PropertyDefinition property = MetadataReader.GetProperty(declaringType, row.Col2);
							if (property.other_methods == null)
							{
								property.other_methods = new Collection<MethodDefinition>();
							}
							property.other_methods.Add(method);
							break;
						}
					}
					default:
					{
						if (col1 == MethodSemanticsAttributes.AddOn)
						{
							MetadataReader.GetEvent(declaringType, row.Col2).add_method = method;
							break;
						}
						else
						{
							throw new NotSupportedException();
						}
					}
				}
			}
			else if (col1 == MethodSemanticsAttributes.RemoveOn)
			{
				MetadataReader.GetEvent(declaringType, row.Col2).remove_method = method;
			}
			else
			{
				if (col1 != MethodSemanticsAttributes.Fire)
				{
					throw new NotSupportedException();
				}
				MetadataReader.GetEvent(declaringType, row.Col2).invoke_method = method;
			}
			this.metadata.Semantics.Remove(method.token.RID);
			return (MethodSemanticsAttributes)row.Col1;
		}

		private void ReadMethodSignature(uint signature, IMethodSignature method)
		{
			this.ReadSignature(signature).ReadMethodSignature(method);
		}

		private MethodSpecification ReadMethodSpecSignature(uint signature, MethodReference method)
		{
			SignatureReader signatureReader = this.ReadSignature(signature);
			if (signatureReader.ReadByte() != 10)
			{
				throw new NotSupportedException();
			}
			GenericInstanceMethod genericInstanceMethod = new GenericInstanceMethod(method);
			signatureReader.ReadGenericInstanceSignature(method, genericInstanceMethod);
			return genericInstanceMethod;
		}

		private Range ReadMethodsRange(uint type_index)
		{
			return this.ReadListRange(type_index, Table.TypeDef, Table.Method);
		}

		public Collection<ModuleReference> ReadModuleReferences()
		{
			this.InitializeModuleReferences();
			return new Collection<ModuleReference>(this.metadata.ModuleReferences);
		}

		public Collection<ModuleDefinition> ReadModules()
		{
			Collection<ModuleDefinition> moduleDefinitions = new Collection<ModuleDefinition>(1)
			{
				this.module
			};
			int num = this.MoveTo(Table.File);
			for (uint i = 1; (ulong)i <= (long)num; i++)
			{
				uint num1 = base.ReadUInt32();
				string str = this.ReadString();
				this.ReadBlobIndex();
				if (num1 == 0)
				{
					ReaderParameters readerParameter = new ReaderParameters()
					{
						ReadingMode = this.module.ReadingMode,
						SymbolReaderProvider = this.module.SymbolReaderProvider,
						AssemblyResolver = this.module.AssemblyResolver
					};
					moduleDefinitions.Add(ModuleDefinition.ReadModule(this.GetModuleFileName(str), readerParameter));
				}
			}
			return moduleDefinitions;
		}

		public Collection<TypeDefinition> ReadNestedTypes(TypeDefinition type)
		{
			uint[] numArray;
			this.InitializeNestedTypes();
			if (!this.metadata.TryGetNestedTypeMapping(type, out numArray))
			{
				return new MemberDefinitionCollection<TypeDefinition>(type);
			}
			MemberDefinitionCollection<TypeDefinition> memberDefinitionCollection = new MemberDefinitionCollection<TypeDefinition>(type, (int)numArray.Length);
			for (int i = 0; i < (int)numArray.Length; i++)
			{
				TypeDefinition typeDefinition = this.GetTypeDefinition(numArray[i]);
				if (typeDefinition != null)
				{
					memberDefinitionCollection.Add(typeDefinition);
				}
			}
			this.metadata.RemoveNestedTypeMapping(type);
			return memberDefinitionCollection;
		}

		public Collection<MethodReference> ReadOverrides(MethodDefinition method)
		{
			MetadataToken[] metadataTokenArray;
			this.InitializeOverrides();
			if (!this.metadata.TryGetOverrideMapping(method, out metadataTokenArray))
			{
				return new Collection<MethodReference>();
			}
			Collection<MethodReference> methodReferences = new Collection<MethodReference>((int)metadataTokenArray.Length);
			this.context = method;
			for (int i = 0; i < (int)metadataTokenArray.Length; i++)
			{
				methodReferences.Add((MethodReference)this.LookupToken(metadataTokenArray[i]));
			}
			this.metadata.RemoveOverrideMapping(method);
			return methodReferences;
		}

		private void ReadParameter(uint param_rid, MethodDefinition method)
		{
			ParameterDefinition metadataToken;
			ParameterAttributes parameterAttribute = (ParameterAttributes)base.ReadUInt16();
			ushort num = base.ReadUInt16();
			string str = this.ReadString();
			metadataToken = (num == 0 ? method.MethodReturnType.Parameter : method.Parameters[num - 1]);
			metadataToken.token = new MetadataToken(Mono.Cecil.TokenType.Param, param_rid);
			metadataToken.Name = str;
			metadataToken.Attributes = parameterAttribute;
		}

		private void ReadParameterPointers(MethodDefinition method, Range range)
		{
			for (uint i = 0; i < range.Length; i++)
			{
				this.MoveTo(Table.ParamPtr, range.Start + i);
				uint num = this.ReadTableIndex(Table.Param);
				this.MoveTo(Table.Param, num);
				this.ReadParameter(num, method);
			}
		}

		private void ReadParameters(MethodDefinition method, Range param_range)
		{
			if (this.MoveTo(Table.ParamPtr, param_range.Start))
			{
				this.ReadParameterPointers(method, param_range);
				return;
			}
			if (!this.MoveTo(Table.Param, param_range.Start))
			{
				return;
			}
			for (uint i = 0; i < param_range.Length; i++)
			{
				this.ReadParameter(param_range.Start + i, method);
			}
		}

		private Range ReadParametersRange(uint method_rid)
		{
			return this.ReadListRange(method_rid, Table.Method, Table.Param);
		}

		public PInvokeInfo ReadPInvokeInfo(MethodDefinition method)
		{
			Row<PInvokeAttributes, uint, uint> row;
			this.InitializePInvokes();
			uint rID = method.token.RID;
			if (!this.metadata.PInvokes.TryGetValue(rID, out row))
			{
				return null;
			}
			this.metadata.PInvokes.Remove(rID);
			return new PInvokeInfo((PInvokeAttributes)row.Col1, this.image.StringHeap.Read(row.Col2), this.module.ModuleReferences[row.Col3 - 1]);
		}

		private void ReadPointers<TMember>(Table ptr, Table table, Range range, Collection<TMember> members, Action<uint, Collection<TMember>> reader)
		where TMember : IMemberDefinition
		{
			for (uint i = 0; i < range.Length; i++)
			{
				this.MoveTo(ptr, range.Start + i);
				uint num = this.ReadTableIndex(table);
				this.MoveTo(table, num);
				reader(num, members);
			}
		}

		public Collection<PropertyDefinition> ReadProperties(TypeDefinition type)
		{
			Range range;
			this.InitializeProperties();
			if (!this.metadata.TryGetPropertiesRange(type, out range))
			{
				return new MemberDefinitionCollection<PropertyDefinition>(type);
			}
			this.metadata.RemovePropertiesRange(type);
			MemberDefinitionCollection<PropertyDefinition> memberDefinitionCollection = new MemberDefinitionCollection<PropertyDefinition>(type, (int)range.Length);
			if (range.Length == 0)
			{
				return memberDefinitionCollection;
			}
			this.context = type;
			if (this.MoveTo(Table.PropertyPtr, range.Start))
			{
				this.ReadPointers<PropertyDefinition>(Table.PropertyPtr, Table.Property, range, memberDefinitionCollection, new Action<uint, Collection<PropertyDefinition>>(this.ReadProperty));
			}
			else
			{
				if (!this.MoveTo(Table.Property, range.Start))
				{
					return memberDefinitionCollection;
				}
				for (uint i = 0; i < range.Length; i++)
				{
					this.ReadProperty(range.Start + i, memberDefinitionCollection);
				}
			}
			return memberDefinitionCollection;
		}

		private Range ReadPropertiesRange(uint rid)
		{
			return this.ReadListRange(rid, Table.PropertyMap, Table.Property);
		}

		private void ReadProperty(uint property_rid, Collection<PropertyDefinition> properties)
		{
			PropertyAttributes propertyAttribute = (PropertyAttributes)base.ReadUInt16();
			string str = this.ReadString();
			SignatureReader signatureReader = this.ReadSignature(this.ReadBlobIndex());
			byte num = signatureReader.ReadByte();
			if ((num & 8) == 0)
			{
				throw new NotSupportedException();
			}
			bool flag = (num & 32) != 0;
			signatureReader.ReadCompressedUInt32();
			PropertyDefinition propertyDefinition = new PropertyDefinition(str, propertyAttribute, signatureReader.ReadTypeSignature())
			{
				HasThis = flag,
				token = new MetadataToken(Mono.Cecil.TokenType.Property, property_rid)
			};
			if (MetadataReader.IsDeleted(propertyDefinition))
			{
				return;
			}
			properties.Add(propertyDefinition);
		}

		public Collection<Resource> ReadResources()
		{
			Resource embeddedResource;
			int num = this.MoveTo(Table.ManifestResource);
			Collection<Resource> resources = new Collection<Resource>(num);
			for (int i = 1; i <= num; i++)
			{
				uint num1 = base.ReadUInt32();
				ManifestResourceAttributes manifestResourceAttribute = (ManifestResourceAttributes)base.ReadUInt32();
				string str = this.ReadString();
				MetadataToken metadataToken = this.ReadMetadataToken(CodedIndex.Implementation);
				if (metadataToken.RID == 0)
				{
					embeddedResource = new EmbeddedResource(str, manifestResourceAttribute, num1, this);
				}
				else if (metadataToken.TokenType != Mono.Cecil.TokenType.AssemblyRef)
				{
					if (metadataToken.TokenType != Mono.Cecil.TokenType.File)
					{
						throw new NotSupportedException();
					}
					Row<Mono.Cecil.FileAttributes, string, uint> row = this.ReadFileRecord(metadataToken.RID);
					embeddedResource = new LinkedResource(str, manifestResourceAttribute)
					{
						File = row.Col2,
						hash = this.ReadBlob(row.Col3)
					};
				}
				else
				{
					embeddedResource = new AssemblyLinkedResource(str, manifestResourceAttribute)
					{
						Assembly = (AssemblyNameReference)this.GetTypeReferenceScope(metadataToken)
					};
				}
				resources.Add(embeddedResource);
			}
			return resources;
		}

		public byte[] ReadSecurityDeclarationBlob(uint signature)
		{
			return this.ReadBlob(signature);
		}

		private void ReadSecurityDeclarationRange(Range range, Collection<SecurityDeclaration> security_declarations)
		{
			if (!this.MoveTo(Table.DeclSecurity, range.Start))
			{
				return;
			}
			for (int i = 0; (long)i < (ulong)range.Length; i++)
			{
				SecurityAction securityAction = (SecurityAction)base.ReadUInt16();
				this.ReadMetadataToken(CodedIndex.HasDeclSecurity);
				uint num = this.ReadBlobIndex();
				security_declarations.Add(new SecurityDeclaration(securityAction, num, this.module));
			}
		}

		public Collection<SecurityDeclaration> ReadSecurityDeclarations(ISecurityDeclarationProvider owner)
		{
			Range[] rangeArray;
			this.InitializeSecurityDeclarations();
			if (!this.metadata.TryGetSecurityDeclarationRanges(owner, out rangeArray))
			{
				return new Collection<SecurityDeclaration>();
			}
			Collection<SecurityDeclaration> securityDeclarations = new Collection<SecurityDeclaration>(MetadataReader.RangesSize(rangeArray));
			for (int i = 0; i < (int)rangeArray.Length; i++)
			{
				this.ReadSecurityDeclarationRange(rangeArray[i], securityDeclarations);
			}
			this.metadata.RemoveSecurityDeclarationRange(owner);
			return securityDeclarations;
		}

		public void ReadSecurityDeclarationSignature(SecurityDeclaration declaration)
		{
			uint num = declaration.signature;
			SignatureReader signatureReader = this.ReadSignature(num);
			if (signatureReader.buffer[signatureReader.position] != 46)
			{
				this.ReadXmlSecurityDeclaration(num, declaration);
				return;
			}
			signatureReader.position++;
			uint num1 = signatureReader.ReadCompressedUInt32();
			Collection<SecurityAttribute> securityAttributes = new Collection<SecurityAttribute>((int)num1);
			for (int i = 0; (long)i < (ulong)num1; i++)
			{
				securityAttributes.Add(signatureReader.ReadSecurityAttribute());
			}
			declaration.security_attributes = securityAttributes;
		}

		private SignatureReader ReadSignature(uint signature)
		{
			return new SignatureReader(signature, this);
		}

		private string ReadString()
		{
			return this.image.StringHeap.Read(this.ReadByIndexSize(this.image.StringHeap.IndexSize));
		}

		private uint ReadStringIndex()
		{
			return this.ReadByIndexSize(this.image.StringHeap.IndexSize);
		}

		private uint ReadTableIndex(Table table)
		{
			return this.ReadByIndexSize(this.image.GetTableIndexSize(table));
		}

		private TypeDefinition ReadType(uint rid)
		{
			if (!this.MoveTo(Table.TypeDef, rid))
			{
				return null;
			}
			TypeAttributes typeAttribute = (TypeAttributes)base.ReadUInt32();
			string str = this.ReadString();
			TypeDefinition typeDefinition = new TypeDefinition(this.ReadString(), str, typeAttribute)
			{
				token = new MetadataToken(Mono.Cecil.TokenType.TypeDef, rid),
				scope = this.module,
				module = this.module
			};
			this.metadata.AddTypeDefinition(typeDefinition);
			this.context = typeDefinition;
			typeDefinition.BaseType = this.GetTypeDefOrRef(this.ReadMetadataToken(CodedIndex.TypeDefOrRef));
			typeDefinition.fields_range = this.ReadFieldsRange(rid);
			typeDefinition.methods_range = this.ReadMethodsRange(rid);
			if (MetadataReader.IsNested(typeAttribute))
			{
				typeDefinition.DeclaringType = this.GetNestedTypeDeclaringType(typeDefinition);
			}
			return typeDefinition;
		}

		private TypeDefinition ReadTypeDefinition(uint rid)
		{
			if (!this.MoveTo(Table.TypeDef, rid))
			{
				return null;
			}
			return this.ReadType(rid);
		}

		public Row<short, int> ReadTypeLayout(TypeDefinition type)
		{
			Row<ushort, uint> row;
			this.InitializeTypeLayouts();
			uint rID = type.token.RID;
			if (!this.metadata.ClassLayouts.TryGetValue(rID, out row))
			{
				return new Row<short, int>(-1, -1);
			}
			type.PackingSize = (short)row.Col1;
			type.ClassSize = row.Col2;
			this.metadata.ClassLayouts.Remove(rID);
			return new Row<short, int>((short)row.Col1, row.Col2);
		}

		private MemberReference ReadTypeMemberReference(MetadataToken type, string name, uint signature)
		{
			TypeReference typeDefOrRef = this.GetTypeDefOrRef(type);
			if (!typeDefOrRef.IsArray)
			{
				this.context = typeDefOrRef;
			}
			MemberReference memberReference = this.ReadMemberReferenceSignature(signature, typeDefOrRef);
			memberReference.Name = name;
			return memberReference;
		}

		private TypeReference ReadTypeReference(uint rid)
		{
			IMetadataScope typeReferenceScope;
			IMetadataScope scope;
			if (!this.MoveTo(Table.TypeRef, rid))
			{
				return null;
			}
			TypeReference typeDefOrRef = null;
			MetadataToken metadataToken = this.ReadMetadataToken(CodedIndex.ResolutionScope);
			string str = this.ReadString();
			TypeReference typeReference = new TypeReference(this.ReadString(), str, this.module, null)
			{
				token = new MetadataToken(Mono.Cecil.TokenType.TypeRef, rid)
			};
			this.metadata.AddTypeReference(typeReference);
			if (metadataToken.TokenType != Mono.Cecil.TokenType.TypeRef)
			{
				typeReferenceScope = this.GetTypeReferenceScope(metadataToken);
			}
			else
			{
				typeDefOrRef = this.GetTypeDefOrRef(metadataToken);
				if (typeDefOrRef != null)
				{
					scope = typeDefOrRef.Scope;
				}
				else
				{
					scope = this.module;
				}
				typeReferenceScope = scope;
			}
			typeReference.scope = typeReferenceScope;
			typeReference.DeclaringType = typeDefOrRef;
			MetadataSystem.TryProcessPrimitiveTypeReference(typeReference);
			return typeReference;
		}

		public TypeDefinitionCollection ReadTypes()
		{
			this.InitializeTypeDefinitions();
			TypeDefinition[] types = this.metadata.Types;
			int length = (int)types.Length - this.metadata.NestedTypes.Count;
			TypeDefinitionCollection typeDefinitionCollection = new TypeDefinitionCollection(this.module, length);
			for (int i = 0; i < (int)types.Length; i++)
			{
				TypeDefinition typeDefinition = types[i];
				if (!MetadataReader.IsNested(typeDefinition.Attributes))
				{
					typeDefinitionCollection.Add(typeDefinition);
				}
			}
			if (this.image.HasTable(Table.MethodPtr) || this.image.HasTable(Table.FieldPtr))
			{
				this.CompleteTypes();
			}
			return typeDefinitionCollection;
		}

		public VariableDefinitionCollection ReadVariables(MetadataToken local_var_token)
		{
			if (!this.MoveTo(Table.StandAloneSig, local_var_token.RID))
			{
				return null;
			}
			SignatureReader signatureReader = this.ReadSignature(this.ReadBlobIndex());
			if (signatureReader.ReadByte() != 7)
			{
				throw new NotSupportedException();
			}
			uint num = signatureReader.ReadCompressedUInt32();
			if (num == 0)
			{
				return null;
			}
			VariableDefinitionCollection variableDefinitionCollection = new VariableDefinitionCollection((int)num);
			for (int i = 0; (long)i < (ulong)num; i++)
			{
				variableDefinitionCollection.Add(new VariableDefinition(signatureReader.ReadTypeSignature()));
			}
			return variableDefinitionCollection;
		}

		private void ReadXmlSecurityDeclaration(uint signature, SecurityDeclaration declaration)
		{
			byte[] numArray = this.ReadBlob(signature);
			Collection<SecurityAttribute> securityAttributes = new Collection<SecurityAttribute>(1);
			SecurityAttribute securityAttribute = new SecurityAttribute(this.module.TypeSystem.LookupType("System.Security.Permissions", "PermissionSetAttribute"))
			{
				properties = new Collection<CustomAttributeNamedArgument>(1)
			};
			securityAttribute.properties.Add(new CustomAttributeNamedArgument("XML", new CustomAttributeArgument(this.module.TypeSystem.String, Encoding.Unicode.GetString(numArray, 0, (int)numArray.Length))));
			securityAttributes.Add(securityAttribute);
			declaration.security_attributes = securityAttributes;
		}
	}
}