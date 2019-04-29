using Mono.Cecil.Metadata;
using Mono.Collections.Generic;
using System;
using System.Runtime.CompilerServices;

namespace Mono.Cecil
{
	public sealed class TypeDefinition : TypeReference, IMemberDefinition, ICustomAttributeProvider, IMetadataTokenProvider, ISecurityDeclarationProvider
	{
		private uint attributes;

		private TypeReference base_type;

		internal Range fields_range;

		internal Range methods_range;

		private short packing_size = -2;

		private int class_size = -2;

		private Collection<TypeReference> interfaces;

		private Collection<TypeDefinition> nested_types;

		private Collection<MethodDefinition> methods;

		private Collection<FieldDefinition> fields;

		private Collection<EventDefinition> events;

		private Collection<PropertyDefinition> properties;

		private Collection<CustomAttribute> custom_attributes;

		private Collection<SecurityDeclaration> security_declarations;

		public TypeAttributes Attributes
		{
			get
			{
				return (TypeAttributes)this.attributes;
			}
			set
			{
				this.attributes = (uint)value;
			}
		}

		public TypeReference BaseType
		{
			get
			{
				return this.base_type;
			}
			set
			{
				this.base_type = value;
			}
		}

		public int ClassSize
		{
			get
			{
				if (this.class_size >= 0)
				{
					return this.class_size;
				}
				this.ResolveLayout();
				if (this.class_size < 0)
				{
					return -1;
				}
				return this.class_size;
			}
			set
			{
				this.class_size = value;
			}
		}

		public Collection<CustomAttribute> CustomAttributes
		{
			get
			{
				return this.custom_attributes ?? this.GetCustomAttributes(ref this.custom_attributes, this.Module);
			}
		}

		public new TypeDefinition DeclaringType
		{
			get
			{
				return (TypeDefinition)base.DeclaringType;
			}
			set
			{
				base.DeclaringType = value;
			}
		}

		public Collection<EventDefinition> Events
		{
			get
			{
				if (this.events != null)
				{
					return this.events;
				}
				if (!base.HasImage)
				{
					MemberDefinitionCollection<EventDefinition> memberDefinitionCollection = new MemberDefinitionCollection<EventDefinition>(this);
					Collection<EventDefinition> eventDefinitions = memberDefinitionCollection;
					this.events = memberDefinitionCollection;
					return eventDefinitions;
				}
				return this.Module.Read<TypeDefinition, Collection<EventDefinition>>(ref this.events, this, (TypeDefinition type, MetadataReader reader) => reader.ReadEvents(type));
			}
		}

		public Collection<FieldDefinition> Fields
		{
			get
			{
				if (this.fields != null)
				{
					return this.fields;
				}
				if (!base.HasImage)
				{
					MemberDefinitionCollection<FieldDefinition> memberDefinitionCollection = new MemberDefinitionCollection<FieldDefinition>(this);
					Collection<FieldDefinition> fieldDefinitions = memberDefinitionCollection;
					this.fields = memberDefinitionCollection;
					return fieldDefinitions;
				}
				return this.Module.Read<TypeDefinition, Collection<FieldDefinition>>(ref this.fields, this, (TypeDefinition type, MetadataReader reader) => reader.ReadFields(type));
			}
		}

		public override Collection<GenericParameter> GenericParameters
		{
			get
			{
				return this.generic_parameters ?? this.GetGenericParameters(ref this.generic_parameters, this.Module);
			}
		}

		public bool HasCustomAttributes
		{
			get
			{
				if (this.custom_attributes == null)
				{
					return this.GetHasCustomAttributes(this.Module);
				}
				return this.custom_attributes.Count > 0;
			}
		}

		public bool HasEvents
		{
			get
			{
				if (this.events != null)
				{
					return this.events.Count > 0;
				}
				if (!base.HasImage)
				{
					return false;
				}
				return this.Module.Read<TypeDefinition, bool>(this, (TypeDefinition type, MetadataReader reader) => reader.HasEvents(type));
			}
		}

		public bool HasFields
		{
			get
			{
				if (this.fields != null)
				{
					return this.fields.Count > 0;
				}
				if (!base.HasImage)
				{
					return false;
				}
				return this.fields_range.Length != 0;
			}
		}

		public override bool HasGenericParameters
		{
			get
			{
				if (this.generic_parameters == null)
				{
					return this.GetHasGenericParameters(this.Module);
				}
				return this.generic_parameters.Count > 0;
			}
		}

		public bool HasInterfaces
		{
			get
			{
				if (this.interfaces != null)
				{
					return this.interfaces.Count > 0;
				}
				if (!base.HasImage)
				{
					return false;
				}
				return this.Module.Read<TypeDefinition, bool>(this, (TypeDefinition type, MetadataReader reader) => reader.HasInterfaces(type));
			}
		}

		public bool HasLayoutInfo
		{
			get
			{
				if (this.packing_size >= 0 || this.class_size >= 0)
				{
					return true;
				}
				this.ResolveLayout();
				if (this.packing_size >= 0)
				{
					return true;
				}
				return this.class_size >= 0;
			}
		}

		public bool HasMethods
		{
			get
			{
				if (this.methods != null)
				{
					return this.methods.Count > 0;
				}
				if (!base.HasImage)
				{
					return false;
				}
				return this.methods_range.Length != 0;
			}
		}

		public bool HasNestedTypes
		{
			get
			{
				if (this.nested_types != null)
				{
					return this.nested_types.Count > 0;
				}
				if (!base.HasImage)
				{
					return false;
				}
				return this.Module.Read<TypeDefinition, bool>(this, (TypeDefinition type, MetadataReader reader) => reader.HasNestedTypes(type));
			}
		}

		public bool HasProperties
		{
			get
			{
				if (this.properties != null)
				{
					return this.properties.Count > 0;
				}
				if (!base.HasImage)
				{
					return false;
				}
				return this.Module.Read<TypeDefinition, bool>(this, (TypeDefinition type, MetadataReader reader) => reader.HasProperties(type));
			}
		}

		public bool HasSecurity
		{
			get
			{
				return this.attributes.GetAttributes(262144);
			}
			set
			{
				this.attributes = this.attributes.SetAttributes(262144, value);
			}
		}

		public bool HasSecurityDeclarations
		{
			get
			{
				if (this.security_declarations == null)
				{
					return this.GetHasSecurityDeclarations(this.Module);
				}
				return this.security_declarations.Count > 0;
			}
		}

		public Collection<TypeReference> Interfaces
		{
			get
			{
				if (this.interfaces != null)
				{
					return this.interfaces;
				}
				if (!base.HasImage)
				{
					Collection<TypeReference> typeReferences = new Collection<TypeReference>();
					Collection<TypeReference> typeReferences1 = typeReferences;
					this.interfaces = typeReferences;
					return typeReferences1;
				}
				return this.Module.Read<TypeDefinition, Collection<TypeReference>>(ref this.interfaces, this, (TypeDefinition type, MetadataReader reader) => reader.ReadInterfaces(type));
			}
		}

		public bool IsAbstract
		{
			get
			{
				return this.attributes.GetAttributes(128);
			}
			set
			{
				this.attributes = this.attributes.SetAttributes(128, value);
			}
		}

		public bool IsAnsiClass
		{
			get
			{
				return this.attributes.GetMaskedAttributes(196608, 0);
			}
			set
			{
				this.attributes = this.attributes.SetMaskedAttributes(196608, 0, value);
			}
		}

		public bool IsAutoClass
		{
			get
			{
				return this.attributes.GetMaskedAttributes(196608, 131072);
			}
			set
			{
				this.attributes = this.attributes.SetMaskedAttributes(196608, 131072, value);
			}
		}

		public bool IsAutoLayout
		{
			get
			{
				return this.attributes.GetMaskedAttributes(24, 0);
			}
			set
			{
				this.attributes = this.attributes.SetMaskedAttributes(24, 0, value);
			}
		}

		public bool IsBeforeFieldInit
		{
			get
			{
				return this.attributes.GetAttributes(1048576);
			}
			set
			{
				this.attributes = this.attributes.SetAttributes(1048576, value);
			}
		}

		public bool IsClass
		{
			get
			{
				return this.attributes.GetMaskedAttributes(32, 0);
			}
			set
			{
				this.attributes = this.attributes.SetMaskedAttributes(32, 0, value);
			}
		}

		public override bool IsDefinition
		{
			get
			{
				return true;
			}
		}

		public bool IsEnum
		{
			get
			{
				if (this.base_type == null)
				{
					return false;
				}
				return this.base_type.IsTypeOf("System", "Enum");
			}
		}

		public bool IsExplicitLayout
		{
			get
			{
				return this.attributes.GetMaskedAttributes(24, 16);
			}
			set
			{
				this.attributes = this.attributes.SetMaskedAttributes(24, 16, value);
			}
		}

		public bool IsImport
		{
			get
			{
				return this.attributes.GetAttributes(4096);
			}
			set
			{
				this.attributes = this.attributes.SetAttributes(4096, value);
			}
		}

		public bool IsInterface
		{
			get
			{
				return this.attributes.GetMaskedAttributes(32, 32);
			}
			set
			{
				this.attributes = this.attributes.SetMaskedAttributes(32, 32, value);
			}
		}

		public bool IsNestedAssembly
		{
			get
			{
				return this.attributes.GetMaskedAttributes(7, 5);
			}
			set
			{
				this.attributes = this.attributes.SetMaskedAttributes(7, 5, value);
			}
		}

		public bool IsNestedFamily
		{
			get
			{
				return this.attributes.GetMaskedAttributes(7, 4);
			}
			set
			{
				this.attributes = this.attributes.SetMaskedAttributes(7, 4, value);
			}
		}

		public bool IsNestedFamilyAndAssembly
		{
			get
			{
				return this.attributes.GetMaskedAttributes(7, 6);
			}
			set
			{
				this.attributes = this.attributes.SetMaskedAttributes(7, 6, value);
			}
		}

		public bool IsNestedFamilyOrAssembly
		{
			get
			{
				return this.attributes.GetMaskedAttributes(7, 7);
			}
			set
			{
				this.attributes = this.attributes.SetMaskedAttributes(7, 7, value);
			}
		}

		public bool IsNestedPrivate
		{
			get
			{
				return this.attributes.GetMaskedAttributes(7, 3);
			}
			set
			{
				this.attributes = this.attributes.SetMaskedAttributes(7, 3, value);
			}
		}

		public bool IsNestedPublic
		{
			get
			{
				return this.attributes.GetMaskedAttributes(7, 2);
			}
			set
			{
				this.attributes = this.attributes.SetMaskedAttributes(7, 2, value);
			}
		}

		public bool IsNotPublic
		{
			get
			{
				return this.attributes.GetMaskedAttributes(7, 0);
			}
			set
			{
				this.attributes = this.attributes.SetMaskedAttributes(7, 0, value);
			}
		}

		public override bool IsPrimitive
		{
			get
			{
				ElementType elementType;
				return MetadataSystem.TryGetPrimitiveElementType(this, out elementType);
			}
		}

		public bool IsPublic
		{
			get
			{
				return this.attributes.GetMaskedAttributes(7, 1);
			}
			set
			{
				this.attributes = this.attributes.SetMaskedAttributes(7, 1, value);
			}
		}

		public bool IsRuntimeSpecialName
		{
			get
			{
				return this.attributes.GetAttributes(2048);
			}
			set
			{
				this.attributes = this.attributes.SetAttributes(2048, value);
			}
		}

		public bool IsSealed
		{
			get
			{
				return this.attributes.GetAttributes(256);
			}
			set
			{
				this.attributes = this.attributes.SetAttributes(256, value);
			}
		}

		public bool IsSequentialLayout
		{
			get
			{
				return this.attributes.GetMaskedAttributes(24, 8);
			}
			set
			{
				this.attributes = this.attributes.SetMaskedAttributes(24, 8, value);
			}
		}

		public bool IsSerializable
		{
			get
			{
				return this.attributes.GetAttributes(8192);
			}
			set
			{
				this.attributes = this.attributes.SetAttributes(8192, value);
			}
		}

		public bool IsSpecialName
		{
			get
			{
				return this.attributes.GetAttributes(1024);
			}
			set
			{
				this.attributes = this.attributes.SetAttributes(1024, value);
			}
		}

		public bool IsUnicodeClass
		{
			get
			{
				return this.attributes.GetMaskedAttributes(196608, 65536);
			}
			set
			{
				this.attributes = this.attributes.SetMaskedAttributes(196608, 65536, value);
			}
		}

		public override bool IsValueType
		{
			get
			{
				if (this.base_type == null)
				{
					return false;
				}
				if (this.base_type.IsTypeOf("System", "Enum"))
				{
					return true;
				}
				if (!this.base_type.IsTypeOf("System", "ValueType"))
				{
					return false;
				}
				return !this.IsTypeOf("System", "Enum");
			}
		}

		public bool IsWindowsRuntime
		{
			get
			{
				return this.attributes.GetAttributes(16384);
			}
			set
			{
				this.attributes = this.attributes.SetAttributes(16384, value);
			}
		}

		public override Mono.Cecil.MetadataType MetadataType
		{
			get
			{
				ElementType elementType;
				if (MetadataSystem.TryGetPrimitiveElementType(this, out elementType))
				{
					return (Mono.Cecil.MetadataType)elementType;
				}
				return base.MetadataType;
			}
		}

		public Collection<MethodDefinition> Methods
		{
			get
			{
				if (this.methods != null)
				{
					return this.methods;
				}
				if (!base.HasImage)
				{
					MemberDefinitionCollection<MethodDefinition> memberDefinitionCollection = new MemberDefinitionCollection<MethodDefinition>(this);
					Collection<MethodDefinition> methodDefinitions = memberDefinitionCollection;
					this.methods = memberDefinitionCollection;
					return methodDefinitions;
				}
				return this.Module.Read<TypeDefinition, Collection<MethodDefinition>>(ref this.methods, this, (TypeDefinition type, MetadataReader reader) => reader.ReadMethods(type));
			}
		}

		public Collection<TypeDefinition> NestedTypes
		{
			get
			{
				if (this.nested_types != null)
				{
					return this.nested_types;
				}
				if (!base.HasImage)
				{
					MemberDefinitionCollection<TypeDefinition> memberDefinitionCollection = new MemberDefinitionCollection<TypeDefinition>(this);
					Collection<TypeDefinition> typeDefinitions = memberDefinitionCollection;
					this.nested_types = memberDefinitionCollection;
					return typeDefinitions;
				}
				return this.Module.Read<TypeDefinition, Collection<TypeDefinition>>(ref this.nested_types, this, (TypeDefinition type, MetadataReader reader) => reader.ReadNestedTypes(type));
			}
		}

		public short PackingSize
		{
			get
			{
				if (this.packing_size >= 0)
				{
					return this.packing_size;
				}
				this.ResolveLayout();
				if (this.packing_size < 0)
				{
					return -1;
				}
				return this.packing_size;
			}
			set
			{
				this.packing_size = value;
			}
		}

		public Collection<PropertyDefinition> Properties
		{
			get
			{
				if (this.properties != null)
				{
					return this.properties;
				}
				if (!base.HasImage)
				{
					MemberDefinitionCollection<PropertyDefinition> memberDefinitionCollection = new MemberDefinitionCollection<PropertyDefinition>(this);
					Collection<PropertyDefinition> propertyDefinitions = memberDefinitionCollection;
					this.properties = memberDefinitionCollection;
					return propertyDefinitions;
				}
				return this.Module.Read<TypeDefinition, Collection<PropertyDefinition>>(ref this.properties, this, (TypeDefinition type, MetadataReader reader) => reader.ReadProperties(type));
			}
		}

		public Collection<SecurityDeclaration> SecurityDeclarations
		{
			get
			{
				return this.security_declarations ?? this.GetSecurityDeclarations(ref this.security_declarations, this.Module);
			}
		}

		public TypeDefinition(string @namespace, string name, TypeAttributes attributes) : base(@namespace, name)
		{
			this.attributes = (uint)attributes;
			this.token = new Mono.Cecil.MetadataToken(Mono.Cecil.TokenType.TypeDef);
		}

		public TypeDefinition(string @namespace, string name, TypeAttributes attributes, TypeReference baseType) : this(@namespace, name, attributes)
		{
			this.BaseType = baseType;
		}

		public override TypeDefinition Resolve()
		{
			return this;
		}

		private void ResolveLayout()
		{
			if (this.packing_size != -2 || this.class_size != -2)
			{
				return;
			}
			if (!base.HasImage)
			{
				this.packing_size = -1;
				this.class_size = -1;
				return;
			}
			Row<short, int> row = this.Module.Read<TypeDefinition, Row<short, int>>(this, (TypeDefinition type, MetadataReader reader) => reader.ReadTypeLayout(type));
			this.packing_size = row.Col1;
			this.class_size = row.Col2;
		}
	}
}