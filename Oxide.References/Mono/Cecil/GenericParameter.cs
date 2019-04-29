using Mono.Cecil.Metadata;
using Mono.Collections.Generic;
using System;
using System.Runtime.CompilerServices;

namespace Mono.Cecil
{
	public sealed class GenericParameter : TypeReference, ICustomAttributeProvider, IMetadataTokenProvider
	{
		internal int position;

		internal Mono.Cecil.GenericParameterType type;

		internal IGenericParameterProvider owner;

		private ushort attributes;

		private Collection<TypeReference> constraints;

		private Collection<CustomAttribute> custom_attributes;

		public GenericParameterAttributes Attributes
		{
			get
			{
				return (GenericParameterAttributes)this.attributes;
			}
			set
			{
				this.attributes = (ushort)value;
			}
		}

		public Collection<TypeReference> Constraints
		{
			get
			{
				if (this.constraints != null)
				{
					return this.constraints;
				}
				if (!base.HasImage)
				{
					Collection<TypeReference> typeReferences = new Collection<TypeReference>();
					Collection<TypeReference> typeReferences1 = typeReferences;
					this.constraints = typeReferences;
					return typeReferences1;
				}
				return this.Module.Read<GenericParameter, Collection<TypeReference>>(ref this.constraints, this, (GenericParameter generic_parameter, MetadataReader reader) => reader.ReadGenericConstraints(generic_parameter));
			}
		}

		public override bool ContainsGenericParameter
		{
			get
			{
				return true;
			}
		}

		public Collection<CustomAttribute> CustomAttributes
		{
			get
			{
				return this.custom_attributes ?? this.GetCustomAttributes(ref this.custom_attributes, this.Module);
			}
		}

		public MethodReference DeclaringMethod
		{
			get
			{
				return this.owner as MethodReference;
			}
		}

		public override TypeReference DeclaringType
		{
			get
			{
				return this.owner as TypeReference;
			}
			set
			{
				throw new InvalidOperationException();
			}
		}

		public override string FullName
		{
			get
			{
				return this.Name;
			}
		}

		public bool HasConstraints
		{
			get
			{
				if (this.constraints != null)
				{
					return this.constraints.Count > 0;
				}
				if (!base.HasImage)
				{
					return false;
				}
				return this.Module.Read<GenericParameter, bool>(this, (GenericParameter generic_parameter, MetadataReader reader) => reader.HasGenericConstraints(generic_parameter));
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

		public bool HasDefaultConstructorConstraint
		{
			get
			{
				return this.attributes.GetAttributes(16);
			}
			set
			{
				this.attributes = this.attributes.SetAttributes(16, value);
			}
		}

		public bool HasNotNullableValueTypeConstraint
		{
			get
			{
				return this.attributes.GetAttributes(8);
			}
			set
			{
				this.attributes = this.attributes.SetAttributes(8, value);
			}
		}

		public bool HasReferenceTypeConstraint
		{
			get
			{
				return this.attributes.GetAttributes(4);
			}
			set
			{
				this.attributes = this.attributes.SetAttributes(4, value);
			}
		}

		public bool IsContravariant
		{
			get
			{
				return this.attributes.GetMaskedAttributes(3, 2);
			}
			set
			{
				this.attributes = this.attributes.SetMaskedAttributes(3, 2, value);
			}
		}

		public bool IsCovariant
		{
			get
			{
				return this.attributes.GetMaskedAttributes(3, 1);
			}
			set
			{
				this.attributes = this.attributes.SetMaskedAttributes(3, 1, value);
			}
		}

		public override bool IsGenericParameter
		{
			get
			{
				return true;
			}
		}

		public bool IsNonVariant
		{
			get
			{
				return this.attributes.GetMaskedAttributes(3, 0);
			}
			set
			{
				this.attributes = this.attributes.SetMaskedAttributes(3, 0, value);
			}
		}

		public override Mono.Cecil.MetadataType MetadataType
		{
			get
			{
				return (Mono.Cecil.MetadataType)this.etype;
			}
		}

		public override ModuleDefinition Module
		{
			get
			{
				return this.module ?? this.owner.Module;
			}
		}

		public override string Name
		{
			get
			{
				if (!string.IsNullOrEmpty(base.Name))
				{
					return base.Name;
				}
				string str = string.Concat((this.type == Mono.Cecil.GenericParameterType.Method ? "!!" : "!"), this.position);
				string str1 = str;
				base.Name = str;
				return str1;
			}
		}

		public override string Namespace
		{
			get
			{
				return string.Empty;
			}
			set
			{
				throw new InvalidOperationException();
			}
		}

		public IGenericParameterProvider Owner
		{
			get
			{
				return this.owner;
			}
		}

		public int Position
		{
			get
			{
				return this.position;
			}
		}

		public override IMetadataScope Scope
		{
			get
			{
				if (this.owner == null)
				{
					return null;
				}
				if (this.owner.GenericParameterType != Mono.Cecil.GenericParameterType.Method)
				{
					return ((TypeReference)this.owner).Scope;
				}
				return ((MethodReference)this.owner).DeclaringType.Scope;
			}
			set
			{
				throw new InvalidOperationException();
			}
		}

		public Mono.Cecil.GenericParameterType Type
		{
			get
			{
				return this.type;
			}
		}

		public GenericParameter(IGenericParameterProvider owner) : this(string.Empty, owner)
		{
		}

		public GenericParameter(string name, IGenericParameterProvider owner) : base(string.Empty, name)
		{
			if (owner == null)
			{
				throw new ArgumentNullException();
			}
			this.position = -1;
			this.owner = owner;
			this.type = owner.GenericParameterType;
			this.etype = GenericParameter.ConvertGenericParameterType(this.type);
			this.token = new Mono.Cecil.MetadataToken(Mono.Cecil.TokenType.GenericParam);
		}

		internal GenericParameter(int position, Mono.Cecil.GenericParameterType type, ModuleDefinition module) : base(string.Empty, string.Empty)
		{
			if (module == null)
			{
				throw new ArgumentNullException();
			}
			this.position = position;
			this.type = type;
			this.etype = GenericParameter.ConvertGenericParameterType(type);
			this.module = module;
			this.token = new Mono.Cecil.MetadataToken(Mono.Cecil.TokenType.GenericParam);
		}

		private static ElementType ConvertGenericParameterType(Mono.Cecil.GenericParameterType type)
		{
			if (type == Mono.Cecil.GenericParameterType.Type)
			{
				return ElementType.Var;
			}
			if (type != Mono.Cecil.GenericParameterType.Method)
			{
				throw new ArgumentOutOfRangeException();
			}
			return ElementType.MVar;
		}

		public override TypeDefinition Resolve()
		{
			return null;
		}
	}
}