using Mono;
using Mono.Collections.Generic;
using System;
using System.Runtime.CompilerServices;

namespace Mono.Cecil
{
	public sealed class FieldDefinition : FieldReference, IMemberDefinition, ICustomAttributeProvider, IMetadataTokenProvider, IConstantProvider, IMarshalInfoProvider
	{
		private ushort attributes;

		private Collection<CustomAttribute> custom_attributes;

		private int offset = -2;

		internal int rva = -2;

		private byte[] initial_value;

		private object constant = Mixin.NotResolved;

		private Mono.Cecil.MarshalInfo marshal_info;

		public FieldAttributes Attributes
		{
			get
			{
				return (FieldAttributes)this.attributes;
			}
			set
			{
				this.attributes = (ushort)value;
			}
		}

		public object Constant
		{
			get
			{
				if (!this.HasConstant)
				{
					return null;
				}
				return this.constant;
			}
			set
			{
				this.constant = value;
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

		public bool HasConstant
		{
			get
			{
				this.ResolveConstant(ref this.constant, this.Module);
				return this.constant != Mixin.NoValue;
			}
			set
			{
				if (!value)
				{
					this.constant = Mixin.NoValue;
				}
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

		public bool HasDefault
		{
			get
			{
				return this.attributes.GetAttributes(32768);
			}
			set
			{
				this.attributes = this.attributes.SetAttributes(32768, value);
			}
		}

		public bool HasLayoutInfo
		{
			get
			{
				if (this.offset >= 0)
				{
					return true;
				}
				this.ResolveLayout();
				return this.offset >= 0;
			}
		}

		public bool HasMarshalInfo
		{
			get
			{
				if (this.marshal_info != null)
				{
					return true;
				}
				return this.GetHasMarshalInfo(this.Module);
			}
		}

		public byte[] InitialValue
		{
			get
			{
				if (this.initial_value != null)
				{
					return this.initial_value;
				}
				this.ResolveRVA();
				if (this.initial_value == null)
				{
					this.initial_value = Empty<byte>.Array;
				}
				return this.initial_value;
			}
			set
			{
				this.initial_value = value;
				this.rva = 0;
			}
		}

		public bool IsAssembly
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

		public bool IsCompilerControlled
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

		public override bool IsDefinition
		{
			get
			{
				return true;
			}
		}

		public bool IsFamily
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

		public bool IsFamilyAndAssembly
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

		public bool IsFamilyOrAssembly
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

		public bool IsInitOnly
		{
			get
			{
				return this.attributes.GetAttributes(32);
			}
			set
			{
				this.attributes = this.attributes.SetAttributes(32, value);
			}
		}

		public bool IsLiteral
		{
			get
			{
				return this.attributes.GetAttributes(64);
			}
			set
			{
				this.attributes = this.attributes.SetAttributes(64, value);
			}
		}

		public bool IsNotSerialized
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

		public bool IsPInvokeImpl
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

		public bool IsPrivate
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

		public bool IsPublic
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

		public bool IsRuntimeSpecialName
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

		public bool IsSpecialName
		{
			get
			{
				return this.attributes.GetAttributes(512);
			}
			set
			{
				this.attributes = this.attributes.SetAttributes(512, value);
			}
		}

		public bool IsStatic
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

		public Mono.Cecil.MarshalInfo MarshalInfo
		{
			get
			{
				return this.marshal_info ?? this.GetMarshalInfo(ref this.marshal_info, this.Module);
			}
			set
			{
				this.marshal_info = value;
			}
		}

		public int Offset
		{
			get
			{
				if (this.offset >= 0)
				{
					return this.offset;
				}
				this.ResolveLayout();
				if (this.offset < 0)
				{
					return -1;
				}
				return this.offset;
			}
			set
			{
				this.offset = value;
			}
		}

		public int RVA
		{
			get
			{
				if (this.rva > 0)
				{
					return this.rva;
				}
				this.ResolveRVA();
				if (this.rva <= 0)
				{
					return 0;
				}
				return this.rva;
			}
		}

		public FieldDefinition(string name, FieldAttributes attributes, TypeReference fieldType) : base(name, fieldType)
		{
			this.attributes = (ushort)attributes;
		}

		public override FieldDefinition Resolve()
		{
			return this;
		}

		private void ResolveLayout()
		{
			if (this.offset != -2)
			{
				return;
			}
			if (!base.HasImage)
			{
				this.offset = -1;
				return;
			}
			this.offset = this.Module.Read<FieldDefinition, int>(this, (FieldDefinition field, MetadataReader reader) => reader.ReadFieldLayout(field));
		}

		private void ResolveRVA()
		{
			if (this.rva != -2)
			{
				return;
			}
			if (!base.HasImage)
			{
				return;
			}
			this.rva = this.Module.Read<FieldDefinition, int>(this, (FieldDefinition field, MetadataReader reader) => reader.ReadFieldRVA(field));
		}
	}
}