using System;

namespace Mono.Cecil
{
	public class ExportedType : IMetadataTokenProvider
	{
		private string @namespace;

		private string name;

		private uint attributes;

		private IMetadataScope scope;

		private ModuleDefinition module;

		private int identifier;

		private ExportedType declaring_type;

		internal Mono.Cecil.MetadataToken token;

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

		public ExportedType DeclaringType
		{
			get
			{
				return this.declaring_type;
			}
			set
			{
				this.declaring_type = value;
			}
		}

		public string FullName
		{
			get
			{
				string str = (string.IsNullOrEmpty(this.@namespace) ? this.name : string.Concat(this.@namespace, ".", this.name));
				if (this.declaring_type == null)
				{
					return str;
				}
				return string.Concat(this.declaring_type.FullName, "/", str);
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

		public int Identifier
		{
			get
			{
				return this.identifier;
			}
			set
			{
				this.identifier = value;
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

		public bool IsForwarder
		{
			get
			{
				return this.attributes.GetAttributes(2097152);
			}
			set
			{
				this.attributes = this.attributes.SetAttributes(2097152, value);
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

		public Mono.Cecil.MetadataToken MetadataToken
		{
			get
			{
				return this.token;
			}
			set
			{
				this.token = value;
			}
		}

		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
			}
		}

		public string Namespace
		{
			get
			{
				return this.@namespace;
			}
			set
			{
				this.@namespace = value;
			}
		}

		public IMetadataScope Scope
		{
			get
			{
				if (this.declaring_type == null)
				{
					return this.scope;
				}
				return this.declaring_type.Scope;
			}
		}

		public ExportedType(string @namespace, string name, ModuleDefinition module, IMetadataScope scope)
		{
			this.@namespace = @namespace;
			this.name = name;
			this.scope = scope;
			this.module = module;
		}

		internal TypeReference CreateReference()
		{
			TypeReference typeReference;
			TypeReference typeReference1 = new TypeReference(this.@namespace, this.name, this.module, this.scope);
			if (this.declaring_type != null)
			{
				typeReference = this.declaring_type.CreateReference();
			}
			else
			{
				typeReference = null;
			}
			typeReference1.DeclaringType = typeReference;
			return typeReference1;
		}

		public TypeDefinition Resolve()
		{
			return this.module.Resolve(this.CreateReference());
		}

		public override string ToString()
		{
			return this.FullName;
		}
	}
}