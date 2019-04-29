using Mono.Cecil.Metadata;
using Mono.Collections.Generic;
using System;

namespace Mono.Cecil
{
	public class TypeReference : MemberReference, IGenericParameterProvider, IMetadataTokenProvider, IGenericContext
	{
		private string @namespace;

		private bool value_type;

		internal IMetadataScope scope;

		internal ModuleDefinition module;

		internal ElementType etype;

		private string fullname;

		protected Collection<GenericParameter> generic_parameters;

		public override TypeReference DeclaringType
		{
			get
			{
				return base.DeclaringType;
			}
			set
			{
				base.DeclaringType = value;
				this.fullname = null;
			}
		}

		public override string FullName
		{
			get
			{
				if (this.fullname != null)
				{
					return this.fullname;
				}
				this.fullname = this.TypeFullName();
				if (this.IsNested)
				{
					this.fullname = string.Concat(this.DeclaringType.FullName, "/", this.fullname);
				}
				return this.fullname;
			}
		}

		public virtual Collection<GenericParameter> GenericParameters
		{
			get
			{
				if (this.generic_parameters != null)
				{
					return this.generic_parameters;
				}
				GenericParameterCollection genericParameterCollection = new GenericParameterCollection(this);
				Collection<GenericParameter> genericParameters = genericParameterCollection;
				this.generic_parameters = genericParameterCollection;
				return genericParameters;
			}
		}

		public virtual bool HasGenericParameters
		{
			get
			{
				return !this.generic_parameters.IsNullOrEmpty<GenericParameter>();
			}
		}

		public virtual bool IsArray
		{
			get
			{
				return false;
			}
		}

		public virtual bool IsByReference
		{
			get
			{
				return false;
			}
		}

		public virtual bool IsFunctionPointer
		{
			get
			{
				return false;
			}
		}

		public virtual bool IsGenericInstance
		{
			get
			{
				return false;
			}
		}

		public virtual bool IsGenericParameter
		{
			get
			{
				return false;
			}
		}

		public bool IsNested
		{
			get
			{
				return this.DeclaringType != null;
			}
		}

		public virtual bool IsOptionalModifier
		{
			get
			{
				return false;
			}
		}

		public virtual bool IsPinned
		{
			get
			{
				return false;
			}
		}

		public virtual bool IsPointer
		{
			get
			{
				return false;
			}
		}

		public virtual bool IsPrimitive
		{
			get
			{
				return this.etype.IsPrimitive();
			}
		}

		public virtual bool IsRequiredModifier
		{
			get
			{
				return false;
			}
		}

		public virtual bool IsSentinel
		{
			get
			{
				return false;
			}
		}

		public virtual bool IsValueType
		{
			get
			{
				return this.value_type;
			}
			set
			{
				this.value_type = value;
			}
		}

		public virtual Mono.Cecil.MetadataType MetadataType
		{
			get
			{
				if (this.etype != ElementType.None)
				{
					return (Mono.Cecil.MetadataType)this.etype;
				}
				if (!this.IsValueType)
				{
					return Mono.Cecil.MetadataType.Class;
				}
				return Mono.Cecil.MetadataType.ValueType;
			}
		}

		public override ModuleDefinition Module
		{
			get
			{
				if (this.module != null)
				{
					return this.module;
				}
				TypeReference declaringType = this.DeclaringType;
				if (declaringType == null)
				{
					return null;
				}
				return declaringType.Module;
			}
		}

		IGenericParameterProvider Mono.Cecil.IGenericContext.Method
		{
			get
			{
				return null;
			}
		}

		IGenericParameterProvider Mono.Cecil.IGenericContext.Type
		{
			get
			{
				return this;
			}
		}

		Mono.Cecil.GenericParameterType Mono.Cecil.IGenericParameterProvider.GenericParameterType
		{
			get
			{
				return Mono.Cecil.GenericParameterType.Type;
			}
		}

		public override string Name
		{
			get
			{
				return base.Name;
			}
			set
			{
				base.Name = value;
				this.fullname = null;
			}
		}

		public virtual string Namespace
		{
			get
			{
				return this.@namespace;
			}
			set
			{
				this.@namespace = value;
				this.fullname = null;
			}
		}

		public virtual IMetadataScope Scope
		{
			get
			{
				TypeReference declaringType = this.DeclaringType;
				if (declaringType != null)
				{
					return declaringType.Scope;
				}
				return this.scope;
			}
			set
			{
				TypeReference declaringType = this.DeclaringType;
				if (declaringType != null)
				{
					declaringType.Scope = value;
					return;
				}
				this.scope = value;
			}
		}

		protected TypeReference(string @namespace, string name) : base(name)
		{
			this.@namespace = @namespace ?? string.Empty;
			this.token = new Mono.Cecil.MetadataToken(Mono.Cecil.TokenType.TypeRef, 0);
		}

		public TypeReference(string @namespace, string name, ModuleDefinition module, IMetadataScope scope) : this(@namespace, name)
		{
			this.module = module;
			this.scope = scope;
		}

		public TypeReference(string @namespace, string name, ModuleDefinition module, IMetadataScope scope, bool valueType) : this(@namespace, name, module, scope)
		{
			this.value_type = valueType;
		}

		public virtual TypeReference GetElementType()
		{
			return this;
		}

		public virtual TypeDefinition Resolve()
		{
			ModuleDefinition module = this.Module;
			if (module == null)
			{
				throw new NotSupportedException();
			}
			return module.Resolve(this);
		}
	}
}