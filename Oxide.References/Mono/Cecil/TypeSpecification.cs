using System;

namespace Mono.Cecil
{
	public abstract class TypeSpecification : TypeReference
	{
		private readonly TypeReference element_type;

		public override bool ContainsGenericParameter
		{
			get
			{
				return this.element_type.ContainsGenericParameter;
			}
		}

		public TypeReference ElementType
		{
			get
			{
				return this.element_type;
			}
		}

		public override string FullName
		{
			get
			{
				return this.element_type.FullName;
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
				return this.element_type.Module;
			}
		}

		public override string Name
		{
			get
			{
				return this.element_type.Name;
			}
			set
			{
				throw new InvalidOperationException();
			}
		}

		public override string Namespace
		{
			get
			{
				return this.element_type.Namespace;
			}
			set
			{
				throw new InvalidOperationException();
			}
		}

		public override IMetadataScope Scope
		{
			get
			{
				return this.element_type.Scope;
			}
			set
			{
				throw new InvalidOperationException();
			}
		}

		internal TypeSpecification(TypeReference type) : base(null, null)
		{
			this.element_type = type;
			this.token = new Mono.Cecil.MetadataToken(Mono.Cecil.TokenType.TypeSpec);
		}

		public override TypeReference GetElementType()
		{
			return this.element_type.GetElementType();
		}
	}
}