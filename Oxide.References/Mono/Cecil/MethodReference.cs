using Mono.Collections.Generic;
using System;
using System.Text;

namespace Mono.Cecil
{
	public class MethodReference : MemberReference, IMethodSignature, IMetadataTokenProvider, IGenericParameterProvider, IGenericContext
	{
		internal ParameterDefinitionCollection parameters;

		private Mono.Cecil.MethodReturnType return_type;

		private bool has_this;

		private bool explicit_this;

		private MethodCallingConvention calling_convention;

		internal Collection<GenericParameter> generic_parameters;

		public virtual MethodCallingConvention CallingConvention
		{
			get
			{
				return this.calling_convention;
			}
			set
			{
				this.calling_convention = value;
			}
		}

		public override bool ContainsGenericParameter
		{
			get
			{
				if (this.ReturnType.ContainsGenericParameter || base.ContainsGenericParameter)
				{
					return true;
				}
				Collection<ParameterDefinition> parameters = this.Parameters;
				for (int i = 0; i < parameters.Count; i++)
				{
					if (parameters[i].ParameterType.ContainsGenericParameter)
					{
						return true;
					}
				}
				return false;
			}
		}

		public virtual bool ExplicitThis
		{
			get
			{
				return this.explicit_this;
			}
			set
			{
				this.explicit_this = value;
			}
		}

		public override string FullName
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(this.ReturnType.FullName).Append(" ").Append(base.MemberFullName());
				this.MethodSignatureFullName(stringBuilder);
				return stringBuilder.ToString();
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

		public virtual bool HasParameters
		{
			get
			{
				return !this.parameters.IsNullOrEmpty<ParameterDefinition>();
			}
		}

		public virtual bool HasThis
		{
			get
			{
				return this.has_this;
			}
			set
			{
				this.has_this = value;
			}
		}

		public virtual bool IsGenericInstance
		{
			get
			{
				return false;
			}
		}

		public virtual Mono.Cecil.MethodReturnType MethodReturnType
		{
			get
			{
				return this.return_type;
			}
			set
			{
				this.return_type = value;
			}
		}

		IGenericParameterProvider Mono.Cecil.IGenericContext.Method
		{
			get
			{
				return this;
			}
		}

		IGenericParameterProvider Mono.Cecil.IGenericContext.Type
		{
			get
			{
				TypeReference declaringType = this.DeclaringType;
				GenericInstanceType genericInstanceType = declaringType as GenericInstanceType;
				if (genericInstanceType == null)
				{
					return declaringType;
				}
				return genericInstanceType.ElementType;
			}
		}

		Mono.Cecil.GenericParameterType Mono.Cecil.IGenericParameterProvider.GenericParameterType
		{
			get
			{
				return Mono.Cecil.GenericParameterType.Method;
			}
		}

		public virtual Collection<ParameterDefinition> Parameters
		{
			get
			{
				if (this.parameters == null)
				{
					this.parameters = new ParameterDefinitionCollection(this);
				}
				return this.parameters;
			}
		}

		public TypeReference ReturnType
		{
			get
			{
				Mono.Cecil.MethodReturnType methodReturnType = this.MethodReturnType;
				if (methodReturnType == null)
				{
					return null;
				}
				return methodReturnType.ReturnType;
			}
			set
			{
				Mono.Cecil.MethodReturnType methodReturnType = this.MethodReturnType;
				if (methodReturnType != null)
				{
					methodReturnType.ReturnType = value;
				}
			}
		}

		internal MethodReference()
		{
			this.return_type = new Mono.Cecil.MethodReturnType(this);
			this.token = new Mono.Cecil.MetadataToken(Mono.Cecil.TokenType.MemberRef);
		}

		public MethodReference(string name, TypeReference returnType) : base(name)
		{
			if (returnType == null)
			{
				throw new ArgumentNullException("returnType");
			}
			this.return_type = new Mono.Cecil.MethodReturnType(this)
			{
				ReturnType = returnType
			};
			this.token = new Mono.Cecil.MetadataToken(Mono.Cecil.TokenType.MemberRef);
		}

		public MethodReference(string name, TypeReference returnType, TypeReference declaringType) : this(name, returnType)
		{
			if (declaringType == null)
			{
				throw new ArgumentNullException("declaringType");
			}
			this.DeclaringType = declaringType;
		}

		public virtual MethodReference GetElementMethod()
		{
			return this;
		}

		public virtual MethodDefinition Resolve()
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