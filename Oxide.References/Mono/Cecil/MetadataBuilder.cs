using Mono.Cecil.Cil;
using Mono.Cecil.Metadata;
using Mono.Cecil.PE;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using System.IO;

namespace Mono.Cecil
{
	internal sealed class MetadataBuilder
	{
		internal readonly ModuleDefinition module;

		internal readonly ISymbolWriterProvider symbol_writer_provider;

		internal readonly ISymbolWriter symbol_writer;

		internal readonly TextMap text_map;

		internal readonly string fq_name;

		private readonly Dictionary<Row<uint, uint, uint>, MetadataToken> type_ref_map;

		private readonly Dictionary<uint, MetadataToken> type_spec_map;

		private readonly Dictionary<Row<uint, uint, uint>, MetadataToken> member_ref_map;

		private readonly Dictionary<Row<uint, uint>, MetadataToken> method_spec_map;

		private readonly Collection<GenericParameter> generic_parameters;

		private readonly Dictionary<MetadataToken, MetadataToken> method_def_map;

		internal readonly CodeWriter code;

		internal readonly DataBuffer data;

		internal readonly ResourceBuffer resources;

		internal readonly StringHeapBuffer string_heap;

		internal readonly UserStringHeapBuffer user_string_heap;

		internal readonly BlobHeapBuffer blob_heap;

		internal readonly TableHeapBuffer table_heap;

		internal MetadataToken entry_point;

		private uint type_rid = 1;

		private uint field_rid = 1;

		private uint method_rid = 1;

		private uint param_rid = 1;

		private uint property_rid = 1;

		private uint event_rid = 1;

		private readonly TypeRefTable type_ref_table;

		private readonly TypeDefTable type_def_table;

		private readonly FieldTable field_table;

		private readonly MethodTable method_table;

		private readonly ParamTable param_table;

		private readonly InterfaceImplTable iface_impl_table;

		private readonly MemberRefTable member_ref_table;

		private readonly ConstantTable constant_table;

		private readonly CustomAttributeTable custom_attribute_table;

		private readonly DeclSecurityTable declsec_table;

		private readonly StandAloneSigTable standalone_sig_table;

		private readonly EventMapTable event_map_table;

		private readonly EventTable event_table;

		private readonly PropertyMapTable property_map_table;

		private readonly PropertyTable property_table;

		private readonly TypeSpecTable typespec_table;

		private readonly MethodSpecTable method_spec_table;

		internal readonly bool write_symbols;

		public MetadataBuilder(ModuleDefinition module, string fq_name, ISymbolWriterProvider symbol_writer_provider, ISymbolWriter symbol_writer)
		{
			this.module = module;
			this.text_map = this.CreateTextMap();
			this.fq_name = fq_name;
			this.symbol_writer_provider = symbol_writer_provider;
			this.symbol_writer = symbol_writer;
			this.write_symbols = symbol_writer != null;
			this.code = new CodeWriter(this);
			this.data = new DataBuffer();
			this.resources = new ResourceBuffer();
			this.string_heap = new StringHeapBuffer();
			this.user_string_heap = new UserStringHeapBuffer();
			this.blob_heap = new BlobHeapBuffer();
			this.table_heap = new TableHeapBuffer(module, this);
			this.type_ref_table = this.GetTable<TypeRefTable>(Table.TypeRef);
			this.type_def_table = this.GetTable<TypeDefTable>(Table.TypeDef);
			this.field_table = this.GetTable<FieldTable>(Table.Field);
			this.method_table = this.GetTable<MethodTable>(Table.Method);
			this.param_table = this.GetTable<ParamTable>(Table.Param);
			this.iface_impl_table = this.GetTable<InterfaceImplTable>(Table.InterfaceImpl);
			this.member_ref_table = this.GetTable<MemberRefTable>(Table.MemberRef);
			this.constant_table = this.GetTable<ConstantTable>(Table.Constant);
			this.custom_attribute_table = this.GetTable<CustomAttributeTable>(Table.CustomAttribute);
			this.declsec_table = this.GetTable<DeclSecurityTable>(Table.DeclSecurity);
			this.standalone_sig_table = this.GetTable<StandAloneSigTable>(Table.StandAloneSig);
			this.event_map_table = this.GetTable<EventMapTable>(Table.EventMap);
			this.event_table = this.GetTable<EventTable>(Table.Event);
			this.property_map_table = this.GetTable<PropertyMapTable>(Table.PropertyMap);
			this.property_table = this.GetTable<PropertyTable>(Table.Property);
			this.typespec_table = this.GetTable<TypeSpecTable>(Table.TypeSpec);
			this.method_spec_table = this.GetTable<MethodSpecTable>(Table.MethodSpec);
			RowEqualityComparer rowEqualityComparer = new RowEqualityComparer();
			this.type_ref_map = new Dictionary<Row<uint, uint, uint>, MetadataToken>(rowEqualityComparer);
			this.type_spec_map = new Dictionary<uint, MetadataToken>();
			this.member_ref_map = new Dictionary<Row<uint, uint, uint>, MetadataToken>(rowEqualityComparer);
			this.method_spec_map = new Dictionary<Row<uint, uint>, MetadataToken>(rowEqualityComparer);
			this.generic_parameters = new Collection<GenericParameter>();
			if (this.write_symbols)
			{
				this.method_def_map = new Dictionary<MetadataToken, MetadataToken>();
			}
		}

		private void AddAssemblyReferences()
		{
			Collection<AssemblyNameReference> assemblyReferences = this.module.AssemblyReferences;
			AssemblyRefTable table = this.GetTable<AssemblyRefTable>(Table.AssemblyRef);
			for (int i = 0; i < assemblyReferences.Count; i++)
			{
				AssemblyNameReference item = assemblyReferences[i];
				byte[] numArray = (item.PublicKey.IsNullOrEmpty<byte>() ? item.PublicKeyToken : item.PublicKey);
				Version version = item.Version;
				int num = table.AddRow(new Row<ushort, ushort, ushort, ushort, AssemblyAttributes, uint, uint, uint, uint>((ushort)version.Major, (ushort)version.Minor, (ushort)version.Build, (ushort)version.Revision, item.Attributes, this.GetBlobIndex(numArray), this.GetStringIndex(item.Name), this.GetStringIndex(item.Culture), this.GetBlobIndex(item.Hash)));
				item.token = new MetadataToken(Mono.Cecil.TokenType.AssemblyRef, num);
			}
		}

		private void AddConstant(IConstantProvider owner, TypeReference type)
		{
			object constant = owner.Constant;
			ElementType constantType = MetadataBuilder.GetConstantType(type, constant);
			this.constant_table.AddRow(new Row<ElementType, uint, uint>(constantType, MetadataBuilder.MakeCodedRID(owner.MetadataToken, CodedIndex.HasConstant), this.GetBlobIndex(this.GetConstantSignature(constantType, constant))));
		}

		private void AddConstraints(GenericParameter generic_parameter, GenericParamConstraintTable table)
		{
			Collection<TypeReference> constraints = generic_parameter.Constraints;
			uint rID = generic_parameter.token.RID;
			for (int i = 0; i < constraints.Count; i++)
			{
				table.AddRow(new Row<uint, uint>(rID, MetadataBuilder.MakeCodedRID(this.GetTypeToken(constraints[i]), CodedIndex.TypeDefOrRef)));
			}
		}

		private void AddCustomAttributes(ICustomAttributeProvider owner)
		{
			Collection<CustomAttribute> customAttributes = owner.CustomAttributes;
			for (int i = 0; i < customAttributes.Count; i++)
			{
				CustomAttribute item = customAttributes[i];
				this.custom_attribute_table.AddRow(new Row<uint, uint, uint>(MetadataBuilder.MakeCodedRID(owner, CodedIndex.HasCustomAttribute), MetadataBuilder.MakeCodedRID(this.LookupToken(item.Constructor), CodedIndex.CustomAttributeType), this.GetBlobIndex(this.GetCustomAttributeSignature(item))));
			}
		}

		private uint AddEmbeddedResource(EmbeddedResource resource)
		{
			return this.resources.AddResource(resource.GetResourceData());
		}

		private void AddEvent(EventDefinition @event)
		{
			this.event_table.AddRow(new Row<EventAttributes, uint, uint>(@event.Attributes, this.GetStringIndex(@event.Name), MetadataBuilder.MakeCodedRID(this.GetTypeToken(@event.EventType), CodedIndex.TypeDefOrRef)));
			uint eventRid = this.event_rid;
			this.event_rid = eventRid + 1;
			@event.token = new MetadataToken(Mono.Cecil.TokenType.Event, eventRid);
			MethodDefinition addMethod = @event.AddMethod;
			if (addMethod != null)
			{
				this.AddSemantic(MethodSemanticsAttributes.AddOn, @event, addMethod);
			}
			addMethod = @event.InvokeMethod;
			if (addMethod != null)
			{
				this.AddSemantic(MethodSemanticsAttributes.Fire, @event, addMethod);
			}
			addMethod = @event.RemoveMethod;
			if (addMethod != null)
			{
				this.AddSemantic(MethodSemanticsAttributes.RemoveOn, @event, addMethod);
			}
			if (@event.HasOtherMethods)
			{
				this.AddOtherSemantic(@event, @event.OtherMethods);
			}
			if (@event.HasCustomAttributes)
			{
				this.AddCustomAttributes(@event);
			}
		}

		private void AddEvents(TypeDefinition type)
		{
			Collection<EventDefinition> events = type.Events;
			this.event_map_table.AddRow(new Row<uint, uint>(type.token.RID, this.event_rid));
			for (int i = 0; i < events.Count; i++)
			{
				this.AddEvent(events[i]);
			}
		}

		private void AddExportedTypes()
		{
			Collection<ExportedType> exportedTypes = this.module.ExportedTypes;
			ExportedTypeTable table = this.GetTable<ExportedTypeTable>(Table.ExportedType);
			for (int i = 0; i < exportedTypes.Count; i++)
			{
				ExportedType item = exportedTypes[i];
				int num = table.AddRow(new Row<TypeAttributes, uint, uint, uint, uint>(item.Attributes, (uint)item.Identifier, this.GetStringIndex(item.Name), this.GetStringIndex(item.Namespace), MetadataBuilder.MakeCodedRID(this.GetExportedTypeScope(item), CodedIndex.Implementation)));
				item.token = new MetadataToken(Mono.Cecil.TokenType.ExportedType, num);
			}
		}

		private void AddField(FieldDefinition field)
		{
			this.field_table.AddRow(new Row<FieldAttributes, uint, uint>(field.Attributes, this.GetStringIndex(field.Name), this.GetBlobIndex(this.GetFieldSignature(field))));
			if (!field.InitialValue.IsNullOrEmpty<byte>())
			{
				this.AddFieldRVA(field);
			}
			if (field.HasLayoutInfo)
			{
				this.AddFieldLayout(field);
			}
			if (field.HasCustomAttributes)
			{
				this.AddCustomAttributes(field);
			}
			if (field.HasConstant)
			{
				this.AddConstant(field, field.FieldType);
			}
			if (field.HasMarshalInfo)
			{
				this.AddMarshalInfo(field);
			}
		}

		private void AddFieldLayout(FieldDefinition field)
		{
			this.GetTable<FieldLayoutTable>(Table.FieldLayout).AddRow(new Row<uint, uint>((uint)field.Offset, field.token.RID));
		}

		private void AddFieldRVA(FieldDefinition field)
		{
			this.GetTable<FieldRVATable>(Table.FieldRVA).AddRow(new Row<uint, uint>(this.data.AddData(field.InitialValue), field.token.RID));
		}

		private void AddFields(TypeDefinition type)
		{
			Collection<FieldDefinition> fields = type.Fields;
			for (int i = 0; i < fields.Count; i++)
			{
				this.AddField(fields[i]);
			}
		}

		private void AddGenericParameters(IGenericParameterProvider owner)
		{
			Collection<GenericParameter> genericParameters = owner.GenericParameters;
			for (int i = 0; i < genericParameters.Count; i++)
			{
				this.generic_parameters.Add(genericParameters[i]);
			}
		}

		private void AddGenericParameters()
		{
			GenericParameter[] genericParameters = this.generic_parameters.items;
			int num = this.generic_parameters.size;
			Array.Sort<GenericParameter>(genericParameters, 0, num, new MetadataBuilder.GenericParameterComparer());
			GenericParamTable table = this.GetTable<GenericParamTable>(Table.GenericParam);
			GenericParamConstraintTable genericParamConstraintTable = this.GetTable<GenericParamConstraintTable>(Table.GenericParamConstraint);
			for (int i = 0; i < num; i++)
			{
				GenericParameter metadataToken = genericParameters[i];
				int num1 = table.AddRow(new Row<ushort, GenericParameterAttributes, uint, uint>((ushort)metadataToken.Position, metadataToken.Attributes, MetadataBuilder.MakeCodedRID(metadataToken.Owner, CodedIndex.TypeOrMethodDef), this.GetStringIndex(metadataToken.Name)));
				metadataToken.token = new MetadataToken(Mono.Cecil.TokenType.GenericParam, num1);
				if (metadataToken.HasConstraints)
				{
					this.AddConstraints(metadataToken, genericParamConstraintTable);
				}
				if (metadataToken.HasCustomAttributes)
				{
					this.AddCustomAttributes(metadataToken);
				}
			}
		}

		private void AddInterfaces(TypeDefinition type)
		{
			Collection<TypeReference> interfaces = type.Interfaces;
			uint rID = type.token.RID;
			for (int i = 0; i < interfaces.Count; i++)
			{
				this.iface_impl_table.AddRow(new Row<uint, uint>(rID, MetadataBuilder.MakeCodedRID(this.GetTypeToken(interfaces[i]), CodedIndex.TypeDefOrRef)));
			}
		}

		private void AddLayoutInfo(TypeDefinition type)
		{
			this.GetTable<ClassLayoutTable>(Table.ClassLayout).AddRow(new Row<ushort, uint, uint>((ushort)type.PackingSize, (uint)type.ClassSize, type.token.RID));
		}

		private uint AddLinkedResource(LinkedResource resource)
		{
			FileTable table = this.GetTable<FileTable>(Table.File);
			byte[] numArray = (resource.Hash.IsNullOrEmpty<byte>() ? CryptoService.ComputeHash(resource.File) : resource.Hash);
			return (uint)table.AddRow(new Row<Mono.Cecil.FileAttributes, uint, uint>(Mono.Cecil.FileAttributes.ContainsNoMetaData, this.GetStringIndex(resource.File), this.GetBlobIndex(numArray)));
		}

		private void AddMarshalInfo(IMarshalInfoProvider owner)
		{
			this.GetTable<FieldMarshalTable>(Table.FieldMarshal).AddRow(new Row<uint, uint>(MetadataBuilder.MakeCodedRID(owner, CodedIndex.HasFieldMarshal), this.GetBlobIndex(this.GetMarshalInfoSignature(owner))));
		}

		private void AddMemberReference(MemberReference member, Row<uint, uint, uint> row)
		{
			member.token = new MetadataToken(Mono.Cecil.TokenType.MemberRef, this.member_ref_table.AddRow(row));
			this.member_ref_map.Add(row, member.token);
		}

		private void AddMethod(MethodDefinition method)
		{
			uint num;
			MethodTable methodTable = this.method_table;
			if (method.HasBody)
			{
				num = this.code.WriteMethodBody(method);
			}
			else
			{
				num = 0;
			}
			methodTable.AddRow(new Row<uint, MethodImplAttributes, MethodAttributes, uint, uint, uint>(num, method.ImplAttributes, method.Attributes, this.GetStringIndex(method.Name), this.GetBlobIndex(this.GetMethodSignature(method)), this.param_rid));
			this.AddParameters(method);
			if (method.HasGenericParameters)
			{
				this.AddGenericParameters(method);
			}
			if (method.IsPInvokeImpl)
			{
				this.AddPInvokeInfo(method);
			}
			if (method.HasCustomAttributes)
			{
				this.AddCustomAttributes(method);
			}
			if (method.HasSecurityDeclarations)
			{
				this.AddSecurityDeclarations(method);
			}
			if (method.HasOverrides)
			{
				this.AddOverrides(method);
			}
		}

		private void AddMethods(TypeDefinition type)
		{
			Collection<MethodDefinition> methods = type.Methods;
			for (int i = 0; i < methods.Count; i++)
			{
				this.AddMethod(methods[i]);
			}
		}

		private void AddMethodSpecification(MethodSpecification method_spec, Row<uint, uint> row)
		{
			method_spec.token = new MetadataToken(Mono.Cecil.TokenType.MethodSpec, this.method_spec_table.AddRow(row));
			this.method_spec_map.Add(row, method_spec.token);
		}

		private void AddModuleReferences()
		{
			Collection<ModuleReference> moduleReferences = this.module.ModuleReferences;
			ModuleRefTable table = this.GetTable<ModuleRefTable>(Table.ModuleRef);
			for (int i = 0; i < moduleReferences.Count; i++)
			{
				ModuleReference item = moduleReferences[i];
				item.token = new MetadataToken(Mono.Cecil.TokenType.ModuleRef, table.AddRow(this.GetStringIndex(item.Name)));
			}
		}

		private void AddNestedTypes(TypeDefinition type)
		{
			Collection<TypeDefinition> nestedTypes = type.NestedTypes;
			NestedClassTable table = this.GetTable<NestedClassTable>(Table.NestedClass);
			for (int i = 0; i < nestedTypes.Count; i++)
			{
				TypeDefinition item = nestedTypes[i];
				this.AddType(item);
				table.AddRow(new Row<uint, uint>(item.token.RID, type.token.RID));
			}
		}

		private void AddOtherSemantic(IMetadataTokenProvider owner, Collection<MethodDefinition> others)
		{
			for (int i = 0; i < others.Count; i++)
			{
				this.AddSemantic(MethodSemanticsAttributes.Other, owner, others[i]);
			}
		}

		private void AddOverrides(MethodDefinition method)
		{
			Collection<MethodReference> overrides = method.Overrides;
			MethodImplTable table = this.GetTable<MethodImplTable>(Table.MethodImpl);
			for (int i = 0; i < overrides.Count; i++)
			{
				table.AddRow(new Row<uint, uint, uint>(method.DeclaringType.token.RID, MetadataBuilder.MakeCodedRID(method, CodedIndex.MethodDefOrRef), MetadataBuilder.MakeCodedRID(this.LookupToken(overrides[i]), CodedIndex.MethodDefOrRef)));
			}
		}

		private void AddParameter(ushort sequence, ParameterDefinition parameter, ParamTable table)
		{
			table.AddRow(new Row<ParameterAttributes, ushort, uint>(parameter.Attributes, sequence, this.GetStringIndex(parameter.Name)));
			uint paramRid = this.param_rid;
			this.param_rid = paramRid + 1;
			parameter.token = new MetadataToken(Mono.Cecil.TokenType.Param, paramRid);
			if (parameter.HasCustomAttributes)
			{
				this.AddCustomAttributes(parameter);
			}
			if (parameter.HasConstant)
			{
				this.AddConstant(parameter, parameter.ParameterType);
			}
			if (parameter.HasMarshalInfo)
			{
				this.AddMarshalInfo(parameter);
			}
		}

		private void AddParameters(MethodDefinition method)
		{
			ParameterDefinition methodReturnType = method.MethodReturnType.parameter;
			if (methodReturnType != null && MetadataBuilder.RequiresParameterRow(methodReturnType))
			{
				this.AddParameter(0, methodReturnType, this.param_table);
			}
			if (!method.HasParameters)
			{
				return;
			}
			Collection<ParameterDefinition> parameters = method.Parameters;
			for (int i = 0; i < parameters.Count; i++)
			{
				ParameterDefinition item = parameters[i];
				if (MetadataBuilder.RequiresParameterRow(item))
				{
					this.AddParameter((ushort)(i + 1), item, this.param_table);
				}
			}
		}

		private void AddPInvokeInfo(MethodDefinition method)
		{
			PInvokeInfo pInvokeInfo = method.PInvokeInfo;
			if (pInvokeInfo == null)
			{
				return;
			}
			ImplMapTable table = this.GetTable<ImplMapTable>(Table.ImplMap);
			PInvokeAttributes attributes = pInvokeInfo.Attributes;
			uint num = MetadataBuilder.MakeCodedRID(method, CodedIndex.MemberForwarded);
			uint stringIndex = this.GetStringIndex(pInvokeInfo.EntryPoint);
			MetadataToken metadataToken = pInvokeInfo.Module.MetadataToken;
			table.AddRow(new Row<PInvokeAttributes, uint, uint, uint>(attributes, num, stringIndex, metadataToken.RID));
		}

		private void AddProperties(TypeDefinition type)
		{
			Collection<PropertyDefinition> properties = type.Properties;
			this.property_map_table.AddRow(new Row<uint, uint>(type.token.RID, this.property_rid));
			for (int i = 0; i < properties.Count; i++)
			{
				this.AddProperty(properties[i]);
			}
		}

		private void AddProperty(PropertyDefinition property)
		{
			this.property_table.AddRow(new Row<PropertyAttributes, uint, uint>(property.Attributes, this.GetStringIndex(property.Name), this.GetBlobIndex(this.GetPropertySignature(property))));
			uint propertyRid = this.property_rid;
			this.property_rid = propertyRid + 1;
			property.token = new MetadataToken(Mono.Cecil.TokenType.Property, propertyRid);
			MethodDefinition getMethod = property.GetMethod;
			if (getMethod != null)
			{
				this.AddSemantic(MethodSemanticsAttributes.Getter, property, getMethod);
			}
			getMethod = property.SetMethod;
			if (getMethod != null)
			{
				this.AddSemantic(MethodSemanticsAttributes.Setter, property, getMethod);
			}
			if (property.HasOtherMethods)
			{
				this.AddOtherSemantic(property, property.OtherMethods);
			}
			if (property.HasCustomAttributes)
			{
				this.AddCustomAttributes(property);
			}
			if (property.HasConstant)
			{
				this.AddConstant(property, property.PropertyType);
			}
		}

		private void AddResources()
		{
			Collection<Resource> resources = this.module.Resources;
			ManifestResourceTable table = this.GetTable<ManifestResourceTable>(Table.ManifestResource);
			for (int i = 0; i < resources.Count; i++)
			{
				Resource item = resources[i];
				Row<uint, ManifestResourceAttributes, uint, uint> row = new Row<uint, ManifestResourceAttributes, uint, uint>(0, item.Attributes, this.GetStringIndex(item.Name), 0);
				switch (item.ResourceType)
				{
					case ResourceType.Linked:
					{
						row.Col4 = CodedIndex.Implementation.CompressMetadataToken(new MetadataToken(Mono.Cecil.TokenType.File, this.AddLinkedResource((LinkedResource)item)));
						break;
					}
					case ResourceType.Embedded:
					{
						row.Col1 = this.AddEmbeddedResource((EmbeddedResource)item);
						break;
					}
					case ResourceType.AssemblyLinked:
					{
						row.Col4 = CodedIndex.Implementation.CompressMetadataToken(((AssemblyLinkedResource)item).Assembly.MetadataToken);
						break;
					}
					default:
					{
						throw new NotSupportedException();
					}
				}
				table.AddRow(row);
			}
		}

		private void AddSecurityDeclarations(ISecurityDeclarationProvider owner)
		{
			Collection<SecurityDeclaration> securityDeclarations = owner.SecurityDeclarations;
			for (int i = 0; i < securityDeclarations.Count; i++)
			{
				SecurityDeclaration item = securityDeclarations[i];
				this.declsec_table.AddRow(new Row<SecurityAction, uint, uint>(item.Action, MetadataBuilder.MakeCodedRID(owner, CodedIndex.HasDeclSecurity), this.GetBlobIndex(this.GetSecurityDeclarationSignature(item))));
			}
		}

		private void AddSemantic(MethodSemanticsAttributes semantics, IMetadataTokenProvider provider, MethodDefinition method)
		{
			method.SemanticsAttributes = semantics;
			this.GetTable<MethodSemanticsTable>(Table.MethodSemantics).AddRow(new Row<MethodSemanticsAttributes, uint, uint>(semantics, method.token.RID, MetadataBuilder.MakeCodedRID(provider, CodedIndex.HasSemantics)));
		}

		public uint AddStandAloneSignature(uint signature)
		{
			return (uint)this.standalone_sig_table.AddRow(signature);
		}

		private void AddType(TypeDefinition type)
		{
			this.type_def_table.AddRow(new Row<TypeAttributes, uint, uint, uint, uint, uint>(type.Attributes, this.GetStringIndex(type.Name), this.GetStringIndex(type.Namespace), MetadataBuilder.MakeCodedRID(this.GetTypeToken(type.BaseType), CodedIndex.TypeDefOrRef), type.fields_range.Start, type.methods_range.Start));
			if (type.HasGenericParameters)
			{
				this.AddGenericParameters(type);
			}
			if (type.HasInterfaces)
			{
				this.AddInterfaces(type);
			}
			if (type.HasLayoutInfo)
			{
				this.AddLayoutInfo(type);
			}
			if (type.HasFields)
			{
				this.AddFields(type);
			}
			if (type.HasMethods)
			{
				this.AddMethods(type);
			}
			if (type.HasProperties)
			{
				this.AddProperties(type);
			}
			if (type.HasEvents)
			{
				this.AddEvents(type);
			}
			if (type.HasCustomAttributes)
			{
				this.AddCustomAttributes(type);
			}
			if (type.HasSecurityDeclarations)
			{
				this.AddSecurityDeclarations(type);
			}
			if (type.HasNestedTypes)
			{
				this.AddNestedTypes(type);
			}
		}

		private void AddTypeDefs()
		{
			Collection<TypeDefinition> types = this.module.Types;
			for (int i = 0; i < types.Count; i++)
			{
				this.AddType(types[i]);
			}
		}

		private MetadataToken AddTypeReference(TypeReference type, Row<uint, uint, uint> row)
		{
			type.token = new MetadataToken(Mono.Cecil.TokenType.TypeRef, this.type_ref_table.AddRow(row));
			MetadataToken metadataToken = type.token;
			this.type_ref_map.Add(row, metadataToken);
			return metadataToken;
		}

		private MetadataToken AddTypeSpecification(TypeReference type, uint row)
		{
			type.token = new MetadataToken(Mono.Cecil.TokenType.TypeSpec, this.typespec_table.AddRow(row));
			MetadataToken metadataToken = type.token;
			this.type_spec_map.Add(row, metadataToken);
			return metadataToken;
		}

		private void AttachFieldsDefToken(TypeDefinition type)
		{
			Collection<FieldDefinition> fields = type.Fields;
			type.fields_range.Length = (uint)fields.Count;
			for (int i = 0; i < fields.Count; i++)
			{
				FieldDefinition item = fields[i];
				uint fieldRid = this.field_rid;
				this.field_rid = fieldRid + 1;
				item.token = new MetadataToken(Mono.Cecil.TokenType.Field, fieldRid);
			}
		}

		private void AttachMethodsDefToken(TypeDefinition type)
		{
			Collection<MethodDefinition> methods = type.Methods;
			type.methods_range.Length = (uint)methods.Count;
			for (int i = 0; i < methods.Count; i++)
			{
				MethodDefinition item = methods[i];
				uint methodRid = this.method_rid;
				this.method_rid = methodRid + 1;
				MetadataToken metadataToken = new MetadataToken(Mono.Cecil.TokenType.Method, methodRid);
				if (this.write_symbols && item.token != MetadataToken.Zero)
				{
					this.method_def_map.Add(metadataToken, item.token);
				}
				item.token = metadataToken;
			}
		}

		private void AttachNestedTypesDefToken(TypeDefinition type)
		{
			Collection<TypeDefinition> nestedTypes = type.NestedTypes;
			for (int i = 0; i < nestedTypes.Count; i++)
			{
				this.AttachTypeDefToken(nestedTypes[i]);
			}
		}

		private void AttachTokens()
		{
			Collection<TypeDefinition> types = this.module.Types;
			for (int i = 0; i < types.Count; i++)
			{
				this.AttachTypeDefToken(types[i]);
			}
		}

		private void AttachTypeDefToken(TypeDefinition type)
		{
			uint typeRid = this.type_rid;
			this.type_rid = typeRid + 1;
			type.token = new MetadataToken(Mono.Cecil.TokenType.TypeDef, typeRid);
			type.fields_range.Start = this.field_rid;
			type.methods_range.Start = this.method_rid;
			if (type.HasFields)
			{
				this.AttachFieldsDefToken(type);
			}
			if (type.HasMethods)
			{
				this.AttachMethodsDefToken(type);
			}
			if (type.HasNestedTypes)
			{
				this.AttachNestedTypesDefToken(type);
			}
		}

		private void BuildAssembly()
		{
			AssemblyDefinition assembly = this.module.Assembly;
			AssemblyNameDefinition name = assembly.Name;
			this.GetTable<AssemblyTable>(Table.Assembly).row = new Row<AssemblyHashAlgorithm, ushort, ushort, ushort, ushort, AssemblyAttributes, uint, uint, uint>(name.HashAlgorithm, (ushort)name.Version.Major, (ushort)name.Version.Minor, (ushort)name.Version.Build, (ushort)name.Version.Revision, name.Attributes, this.GetBlobIndex(name.PublicKey), this.GetStringIndex(name.Name), this.GetStringIndex(name.Culture));
			if (assembly.Modules.Count > 1)
			{
				this.BuildModules();
			}
		}

		public void BuildMetadata()
		{
			this.BuildModule();
			this.table_heap.WriteTableHeap();
		}

		private void BuildModule()
		{
			this.GetTable<ModuleTable>(Table.Module).row = this.GetStringIndex(this.module.Name);
			AssemblyDefinition assembly = this.module.Assembly;
			if (assembly != null)
			{
				this.BuildAssembly();
			}
			if (this.module.HasAssemblyReferences)
			{
				this.AddAssemblyReferences();
			}
			if (this.module.HasModuleReferences)
			{
				this.AddModuleReferences();
			}
			if (this.module.HasResources)
			{
				this.AddResources();
			}
			if (this.module.HasExportedTypes)
			{
				this.AddExportedTypes();
			}
			this.BuildTypes();
			if (assembly != null)
			{
				if (assembly.HasCustomAttributes)
				{
					this.AddCustomAttributes(assembly);
				}
				if (assembly.HasSecurityDeclarations)
				{
					this.AddSecurityDeclarations(assembly);
				}
			}
			if (this.module.HasCustomAttributes)
			{
				this.AddCustomAttributes(this.module);
			}
			if (this.module.EntryPoint != null)
			{
				this.entry_point = this.LookupToken(this.module.EntryPoint);
			}
		}

		private void BuildModules()
		{
			Collection<ModuleDefinition> modules = this.module.Assembly.Modules;
			FileTable table = this.GetTable<FileTable>(Table.File);
			for (int i = 0; i < modules.Count; i++)
			{
				ModuleDefinition item = modules[i];
				if (!item.IsMain)
				{
					WriterParameters writerParameter = new WriterParameters()
					{
						SymbolWriterProvider = this.symbol_writer_provider
					};
					string moduleFileName = this.GetModuleFileName(item.Name);
					item.Write(moduleFileName, writerParameter);
					byte[] numArray = CryptoService.ComputeHash(moduleFileName);
					table.AddRow(new Row<Mono.Cecil.FileAttributes, uint, uint>(Mono.Cecil.FileAttributes.ContainsMetaData, this.GetStringIndex(item.Name), this.GetBlobIndex(numArray)));
				}
			}
		}

		private void BuildTypes()
		{
			if (!this.module.HasTypes)
			{
				return;
			}
			this.AttachTokens();
			this.AddTypeDefs();
			this.AddGenericParameters();
		}

		private static Exception CreateForeignMemberException(MemberReference member)
		{
			return new ArgumentException(string.Format("Member '{0}' is declared in another module and needs to be imported", member));
		}

		private Row<uint, uint, uint> CreateMemberRefRow(MemberReference member)
		{
			return new Row<uint, uint, uint>(MetadataBuilder.MakeCodedRID(this.GetTypeToken(member.DeclaringType), CodedIndex.MemberRefParent), this.GetStringIndex(member.Name), this.GetBlobIndex(this.GetMemberRefSignature(member)));
		}

		private Row<uint, uint> CreateMethodSpecRow(MethodSpecification method_spec)
		{
			return new Row<uint, uint>(MetadataBuilder.MakeCodedRID(this.LookupToken(method_spec.ElementMethod), CodedIndex.MethodDefOrRef), this.GetBlobIndex(this.GetMethodSpecSignature(method_spec)));
		}

		private SignatureWriter CreateSignatureWriter()
		{
			return new SignatureWriter(this);
		}

		private TextMap CreateTextMap()
		{
			TextMap textMap = new TextMap();
			textMap.AddMap(TextSegment.ImportAddressTable, (this.module.Architecture == TargetArchitecture.I386 ? 8 : 0));
			textMap.AddMap(TextSegment.CLIHeader, 72, 8);
			return textMap;
		}

		private Row<uint, uint, uint> CreateTypeRefRow(TypeReference type)
		{
			return new Row<uint, uint, uint>(MetadataBuilder.MakeCodedRID(this.GetScopeToken(type), CodedIndex.ResolutionScope), this.GetStringIndex(type.Name), this.GetStringIndex(type.Namespace));
		}

		private uint GetBlobIndex(ByteBuffer blob)
		{
			if (blob.length == 0)
			{
				return (uint)0;
			}
			return this.blob_heap.GetBlobIndex(blob);
		}

		private uint GetBlobIndex(byte[] blob)
		{
			if (blob.IsNullOrEmpty<byte>())
			{
				return (uint)0;
			}
			return this.GetBlobIndex(new ByteBuffer(blob));
		}

		public uint GetCallSiteBlobIndex(CallSite call_site)
		{
			return this.GetBlobIndex(this.GetMethodSignature(call_site));
		}

		private SignatureWriter GetConstantSignature(ElementType type, object value)
		{
			SignatureWriter signatureWriter = this.CreateSignatureWriter();
			switch (type)
			{
				case ElementType.String:
				{
					signatureWriter.WriteConstantString((string)value);
					break;
				}
				case ElementType.Ptr:
				case ElementType.ByRef:
				case ElementType.ValueType:
				{
				Label1:
					signatureWriter.WriteConstantPrimitive(value);
					break;
				}
				case ElementType.Class:
				case ElementType.Var:
				case ElementType.Array:
				{
				Label0:
					signatureWriter.WriteInt32(0);
					break;
				}
				default:
				{
					switch (type)
					{
						case ElementType.Object:
						case ElementType.SzArray:
						case ElementType.MVar:
						{
							goto Label0;
						}
						default:
						{
							goto Label1;
						}
					}
					break;
				}
			}
			return signatureWriter;
		}

		private static ElementType GetConstantType(TypeReference constant_type, object constant)
		{
			if (constant == null)
			{
				return ElementType.Class;
			}
			ElementType constantType = constant_type.etype;
			switch (constantType)
			{
				case ElementType.None:
				{
					TypeDefinition typeDefinition = constant_type.CheckedResolve();
					if (!typeDefinition.IsEnum)
					{
						return ElementType.Class;
					}
					return MetadataBuilder.GetConstantType(typeDefinition.GetEnumUnderlyingType(), constant);
				}
				case ElementType.Void:
				case ElementType.Ptr:
				case ElementType.ValueType:
				case ElementType.Class:
				case ElementType.TypedByRef:
				case ElementType.Void | ElementType.Boolean | ElementType.Char | ElementType.I1 | ElementType.U1 | ElementType.I2 | ElementType.U2 | ElementType.ByRef | ElementType.ValueType | ElementType.Class | ElementType.Var | ElementType.Array | ElementType.GenericInst | ElementType.TypedByRef:
				case ElementType.Boolean | ElementType.I4 | ElementType.I8 | ElementType.ByRef | ElementType.Class | ElementType.I:
				case ElementType.FnPtr:
				{
					return constantType;
				}
				case ElementType.Boolean:
				case ElementType.Char:
				case ElementType.I1:
				case ElementType.U1:
				case ElementType.I2:
				case ElementType.U2:
				case ElementType.I4:
				case ElementType.U4:
				case ElementType.I8:
				case ElementType.U8:
				case ElementType.R4:
				case ElementType.R8:
				case ElementType.I:
				case ElementType.U:
				{
					return MetadataBuilder.GetConstantType(constant.GetType());
				}
				case ElementType.String:
				{
					return ElementType.String;
				}
				case ElementType.ByRef:
				case ElementType.CModReqD:
				case ElementType.CModOpt:
				{
					return MetadataBuilder.GetConstantType(((TypeSpecification)constant_type).ElementType, constant);
				}
				case ElementType.Var:
				case ElementType.Array:
				case ElementType.SzArray:
				case ElementType.MVar:
				{
					return ElementType.Class;
				}
				case ElementType.GenericInst:
				{
					GenericInstanceType genericInstanceType = (GenericInstanceType)constant_type;
					if (!genericInstanceType.ElementType.IsTypeOf("System", "Nullable`1"))
					{
						return MetadataBuilder.GetConstantType(((TypeSpecification)constant_type).ElementType, constant);
					}
					return MetadataBuilder.GetConstantType(genericInstanceType.GenericArguments[0], constant);
				}
				case ElementType.Object:
				{
					return MetadataBuilder.GetConstantType(constant.GetType());
				}
				default:
				{
					if (constantType == ElementType.Sentinel)
					{
						return MetadataBuilder.GetConstantType(((TypeSpecification)constant_type).ElementType, constant);
					}
					return constantType;
				}
			}
		}

		private static ElementType GetConstantType(Type type)
		{
			switch (Type.GetTypeCode(type))
			{
				case TypeCode.Boolean:
				{
					return ElementType.Boolean;
				}
				case TypeCode.Char:
				{
					return ElementType.Char;
				}
				case TypeCode.SByte:
				{
					return ElementType.I1;
				}
				case TypeCode.Byte:
				{
					return ElementType.U1;
				}
				case TypeCode.Int16:
				{
					return ElementType.I2;
				}
				case TypeCode.UInt16:
				{
					return ElementType.U2;
				}
				case TypeCode.Int32:
				{
					return ElementType.I4;
				}
				case TypeCode.UInt32:
				{
					return ElementType.U4;
				}
				case TypeCode.Int64:
				{
					return ElementType.I8;
				}
				case TypeCode.UInt64:
				{
					return ElementType.U8;
				}
				case TypeCode.Single:
				{
					return ElementType.R4;
				}
				case TypeCode.Double:
				{
					return ElementType.R8;
				}
				case TypeCode.Decimal:
				case TypeCode.DateTime:
				case TypeCode.Object | TypeCode.DateTime:
				{
					throw new NotSupportedException(type.FullName);
				}
				case TypeCode.String:
				{
					return ElementType.String;
				}
				default:
				{
					throw new NotSupportedException(type.FullName);
				}
			}
		}

		private SignatureWriter GetCustomAttributeSignature(CustomAttribute attribute)
		{
			SignatureWriter signatureWriter = this.CreateSignatureWriter();
			if (!attribute.resolved)
			{
				signatureWriter.WriteBytes(attribute.GetBlob());
				return signatureWriter;
			}
			signatureWriter.WriteUInt16(1);
			signatureWriter.WriteCustomAttributeConstructorArguments(attribute);
			signatureWriter.WriteCustomAttributeNamedArguments(attribute);
			return signatureWriter;
		}

		private MetadataToken GetExportedTypeScope(ExportedType exported_type)
		{
			if (exported_type.DeclaringType != null)
			{
				return exported_type.DeclaringType.MetadataToken;
			}
			IMetadataScope scope = exported_type.Scope;
			Mono.Cecil.TokenType tokenType = scope.MetadataToken.TokenType;
			if (tokenType == Mono.Cecil.TokenType.ModuleRef)
			{
				FileTable table = this.GetTable<FileTable>(Table.File);
				for (int i = 0; i < table.length; i++)
				{
					if (table.rows[i].Col2 == this.GetStringIndex(scope.Name))
					{
						return new MetadataToken(Mono.Cecil.TokenType.File, i + 1);
					}
				}
			}
			else if (tokenType == Mono.Cecil.TokenType.AssemblyRef)
			{
				return scope.MetadataToken;
			}
			throw new NotSupportedException();
		}

		private SignatureWriter GetFieldSignature(FieldReference field)
		{
			SignatureWriter signatureWriter = this.CreateSignatureWriter();
			signatureWriter.WriteByte(6);
			signatureWriter.WriteTypeSignature(field.FieldType);
			return signatureWriter;
		}

		public uint GetLocalVariableBlobIndex(Collection<VariableDefinition> variables)
		{
			return this.GetBlobIndex(this.GetVariablesSignature(variables));
		}

		private SignatureWriter GetMarshalInfoSignature(IMarshalInfoProvider owner)
		{
			SignatureWriter signatureWriter = this.CreateSignatureWriter();
			signatureWriter.WriteMarshalInfo(owner.MarshalInfo);
			return signatureWriter;
		}

		private SignatureWriter GetMemberRefSignature(MemberReference member)
		{
			FieldReference fieldReference = member as FieldReference;
			if (fieldReference != null)
			{
				return this.GetFieldSignature(fieldReference);
			}
			MethodReference methodReference = member as MethodReference;
			if (methodReference == null)
			{
				throw new NotSupportedException();
			}
			return this.GetMethodSignature(methodReference);
		}

		private MetadataToken GetMemberRefToken(MemberReference member)
		{
			MetadataToken metadataToken;
			Row<uint, uint, uint> row = this.CreateMemberRefRow(member);
			if (this.member_ref_map.TryGetValue(row, out metadataToken))
			{
				return metadataToken;
			}
			this.AddMemberReference(member, row);
			return member.token;
		}

		private SignatureWriter GetMethodSignature(IMethodSignature method)
		{
			SignatureWriter signatureWriter = this.CreateSignatureWriter();
			signatureWriter.WriteMethodSignature(method);
			return signatureWriter;
		}

		private SignatureWriter GetMethodSpecSignature(MethodSpecification method_spec)
		{
			if (!method_spec.IsGenericInstance)
			{
				throw new NotSupportedException();
			}
			GenericInstanceMethod methodSpec = (GenericInstanceMethod)method_spec;
			SignatureWriter signatureWriter = this.CreateSignatureWriter();
			signatureWriter.WriteByte(10);
			signatureWriter.WriteGenericInstanceSignature(methodSpec);
			return signatureWriter;
		}

		private MetadataToken GetMethodSpecToken(MethodSpecification method_spec)
		{
			MetadataToken metadataToken;
			Row<uint, uint> row = this.CreateMethodSpecRow(method_spec);
			if (this.method_spec_map.TryGetValue(row, out metadataToken))
			{
				return metadataToken;
			}
			this.AddMethodSpecification(method_spec, row);
			return method_spec.token;
		}

		private string GetModuleFileName(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new NotSupportedException();
			}
			return Path.Combine(Path.GetDirectoryName(this.fq_name), name);
		}

		private SignatureWriter GetPropertySignature(PropertyDefinition property)
		{
			SignatureWriter signatureWriter = this.CreateSignatureWriter();
			byte num = 8;
			if (property.HasThis)
			{
				num = (byte)(num | 32);
			}
			uint count = 0;
			Collection<ParameterDefinition> parameters = null;
			if (property.HasParameters)
			{
				parameters = property.Parameters;
				count = (uint)parameters.Count;
			}
			signatureWriter.WriteByte(num);
			signatureWriter.WriteCompressedUInt32(count);
			signatureWriter.WriteTypeSignature(property.PropertyType);
			if (count == 0)
			{
				return signatureWriter;
			}
			for (int i = 0; (long)i < (ulong)count; i++)
			{
				signatureWriter.WriteTypeSignature(parameters[i].ParameterType);
			}
			return signatureWriter;
		}

		private MetadataToken GetScopeToken(TypeReference type)
		{
			if (type.IsNested)
			{
				return this.GetTypeRefToken(type.DeclaringType);
			}
			IMetadataScope scope = type.Scope;
			if (scope == null)
			{
				return MetadataToken.Zero;
			}
			return scope.MetadataToken;
		}

		private SignatureWriter GetSecurityDeclarationSignature(SecurityDeclaration declaration)
		{
			SignatureWriter signatureWriter = this.CreateSignatureWriter();
			if (!declaration.resolved)
			{
				signatureWriter.WriteBytes(declaration.GetBlob());
			}
			else if (this.module.Runtime >= TargetRuntime.Net_2_0)
			{
				signatureWriter.WriteSecurityDeclaration(declaration);
			}
			else
			{
				signatureWriter.WriteXmlSecurityDeclaration(declaration);
			}
			return signatureWriter;
		}

		private uint GetStringIndex(string @string)
		{
			if (string.IsNullOrEmpty(@string))
			{
				return (uint)0;
			}
			return this.string_heap.GetStringIndex(@string);
		}

		private TTable GetTable<TTable>(Table table)
		where TTable : MetadataTable, new()
		{
			return this.table_heap.GetTable<TTable>(table);
		}

		private MetadataToken GetTypeRefToken(TypeReference type)
		{
			MetadataToken metadataToken;
			Row<uint, uint, uint> row = this.CreateTypeRefRow(type);
			if (this.type_ref_map.TryGetValue(row, out metadataToken))
			{
				return metadataToken;
			}
			return this.AddTypeReference(type, row);
		}

		private SignatureWriter GetTypeSpecSignature(TypeReference type)
		{
			SignatureWriter signatureWriter = this.CreateSignatureWriter();
			signatureWriter.WriteTypeSignature(type);
			return signatureWriter;
		}

		private MetadataToken GetTypeSpecToken(TypeReference type)
		{
			MetadataToken metadataToken;
			uint blobIndex = this.GetBlobIndex(this.GetTypeSpecSignature(type));
			if (this.type_spec_map.TryGetValue(blobIndex, out metadataToken))
			{
				return metadataToken;
			}
			return this.AddTypeSpecification(type, blobIndex);
		}

		private MetadataToken GetTypeToken(TypeReference type)
		{
			if (type == null)
			{
				return MetadataToken.Zero;
			}
			if (type.IsDefinition)
			{
				return type.token;
			}
			if (type.IsTypeSpecification())
			{
				return this.GetTypeSpecToken(type);
			}
			return this.GetTypeRefToken(type);
		}

		private SignatureWriter GetVariablesSignature(Collection<VariableDefinition> variables)
		{
			SignatureWriter signatureWriter = this.CreateSignatureWriter();
			signatureWriter.WriteByte(7);
			signatureWriter.WriteCompressedUInt32((uint)variables.Count);
			for (int i = 0; i < variables.Count; i++)
			{
				signatureWriter.WriteTypeSignature(variables[i].VariableType);
			}
			return signatureWriter;
		}

		public MetadataToken LookupToken(IMetadataTokenProvider provider)
		{
			if (provider == null)
			{
				throw new ArgumentNullException();
			}
			MemberReference memberReference = provider as MemberReference;
			if (memberReference == null || memberReference.Module != this.module)
			{
				throw MetadataBuilder.CreateForeignMemberException(memberReference);
			}
			MetadataToken metadataToken = provider.MetadataToken;
			Mono.Cecil.TokenType tokenType = metadataToken.TokenType;
			if (tokenType <= Mono.Cecil.TokenType.MemberRef)
			{
				if (tokenType > Mono.Cecil.TokenType.TypeDef)
				{
					if (tokenType == Mono.Cecil.TokenType.Field || tokenType == Mono.Cecil.TokenType.Method)
					{
						return metadataToken;
					}
					if (tokenType == Mono.Cecil.TokenType.MemberRef)
					{
						return this.GetMemberRefToken(memberReference);
					}
					throw new NotSupportedException();
				}
				else
				{
					if (tokenType == Mono.Cecil.TokenType.TypeRef)
					{
						return this.GetTypeToken((TypeReference)provider);
					}
					if (tokenType == Mono.Cecil.TokenType.TypeDef)
					{
						return metadataToken;
					}
					throw new NotSupportedException();
				}
			}
			else if (tokenType > Mono.Cecil.TokenType.Property)
			{
				if (tokenType == Mono.Cecil.TokenType.TypeSpec || tokenType == Mono.Cecil.TokenType.GenericParam)
				{
					return this.GetTypeToken((TypeReference)provider);
				}
				if (tokenType == Mono.Cecil.TokenType.MethodSpec)
				{
					return this.GetMethodSpecToken((MethodSpecification)provider);
				}
				throw new NotSupportedException();
			}
			else
			{
				if (tokenType == Mono.Cecil.TokenType.Event || tokenType == Mono.Cecil.TokenType.Property)
				{
					return metadataToken;
				}
				throw new NotSupportedException();
			}
			return metadataToken;
		}

		private static uint MakeCodedRID(IMetadataTokenProvider provider, CodedIndex index)
		{
			return MetadataBuilder.MakeCodedRID(provider.MetadataToken, index);
		}

		private static uint MakeCodedRID(MetadataToken token, CodedIndex index)
		{
			return index.CompressMetadataToken(token);
		}

		private static bool RequiresParameterRow(ParameterDefinition parameter)
		{
			if (!string.IsNullOrEmpty(parameter.Name) || parameter.Attributes != ParameterAttributes.None || parameter.HasMarshalInfo || parameter.HasConstant)
			{
				return true;
			}
			return parameter.HasCustomAttributes;
		}

		public bool TryGetOriginalMethodToken(MetadataToken new_token, out MetadataToken original)
		{
			return this.method_def_map.TryGetValue(new_token, out original);
		}

		private sealed class GenericParameterComparer : IComparer<GenericParameter>
		{
			public GenericParameterComparer()
			{
			}

			public int Compare(GenericParameter a, GenericParameter b)
			{
				uint num = MetadataBuilder.MakeCodedRID(a.Owner, CodedIndex.TypeOrMethodDef);
				uint num1 = MetadataBuilder.MakeCodedRID(b.Owner, CodedIndex.TypeOrMethodDef);
				if (num != num1)
				{
					if (num <= num1)
					{
						return -1;
					}
					return 1;
				}
				int position = a.Position;
				int position1 = b.Position;
				if (position == position1)
				{
					return 0;
				}
				if (position <= position1)
				{
					return -1;
				}
				return 1;
			}
		}
	}
}