using Mono.Collections.Generic;
using System;
using System.Text;

namespace Mono.Cecil
{
	public sealed class GenericInstanceMethod : MethodSpecification, IGenericInstance, IMetadataTokenProvider, IGenericContext
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

		public override string FullName
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				MethodReference elementMethod = base.ElementMethod;
				stringBuilder.Append(elementMethod.ReturnType.FullName).Append(" ").Append(elementMethod.DeclaringType.FullName).Append("::").Append(elementMethod.Name);
				this.GenericInstanceFullName(stringBuilder);
				this.MethodSignatureFullName(stringBuilder);
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

		IGenericParameterProvider Mono.Cecil.IGenericContext.Method
		{
			get
			{
				return base.ElementMethod;
			}
		}

		IGenericParameterProvider Mono.Cecil.IGenericContext.Type
		{
			get
			{
				return base.ElementMethod.DeclaringType;
			}
		}

		public GenericInstanceMethod(MethodReference method) : base(method)
		{
		}
	}
}