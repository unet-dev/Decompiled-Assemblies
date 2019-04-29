using Mono.Cecil.Metadata;
using Mono.Collections.Generic;
using System;
using System.Text;

namespace Mono.Cecil
{
	public sealed class GenericInstanceType : TypeSpecification, IGenericInstance, IMetadataTokenProvider, IGenericContext
	{
		private Collection<TypeReference> arguments;

		public override bool ContainsGenericParameter
		{
			get
			{
				if (this.ContainsGenericParameter())
				{
					return true;
				}
				return base.ContainsGenericParameter;
			}
		}

		public override TypeReference DeclaringType
		{
			get
			{
				return base.ElementType.DeclaringType;
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		public override string FullName
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(base.FullName);
				this.GenericInstanceFullName(stringBuilder);
				return stringBuilder.ToString();
			}
		}

		public Collection<TypeReference> GenericArguments
		{
			get
			{
				Collection<TypeReference> typeReferences = this.arguments;
				if (typeReferences == null)
				{
					Collection<TypeReference> typeReferences1 = new Collection<TypeReference>();
					Collection<TypeReference> typeReferences2 = typeReferences1;
					this.arguments = typeReferences1;
					typeReferences = typeReferences2;
				}
				return typeReferences;
			}
		}

		public bool HasGenericArguments
		{
			get
			{
				return !this.arguments.IsNullOrEmpty<TypeReference>();
			}
		}

		public override bool IsGenericInstance
		{
			get
			{
				return true;
			}
		}

		IGenericParameterProvider Mono.Cecil.IGenericContext.Type
		{
			get
			{
				return base.ElementType;
			}
		}

		public GenericInstanceType(TypeReference type) : base(type)
		{
			base.IsValueType = type.IsValueType;
			this.etype = Mono.Cecil.Metadata.ElementType.GenericInst;
		}
	}
}