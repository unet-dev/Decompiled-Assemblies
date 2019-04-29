using Mono.Collections.Generic;
using System;

namespace Mono.Cecil
{
	public abstract class MethodSpecification : MethodReference
	{
		private readonly MethodReference method;

		public override MethodCallingConvention CallingConvention
		{
			get
			{
				return this.method.CallingConvention;
			}
			set
			{
				throw new InvalidOperationException();
			}
		}

		public override bool ContainsGenericParameter
		{
			get
			{
				return this.method.ContainsGenericParameter;
			}
		}

		public override TypeReference DeclaringType
		{
			get
			{
				return this.method.DeclaringType;
			}
			set
			{
				throw new InvalidOperationException();
			}
		}

		public MethodReference ElementMethod
		{
			get
			{
				return this.method;
			}
		}

		public override bool ExplicitThis
		{
			get
			{
				return this.method.ExplicitThis;
			}
			set
			{
				throw new InvalidOperationException();
			}
		}

		public override bool HasParameters
		{
			get
			{
				return this.method.HasParameters;
			}
		}

		public override bool HasThis
		{
			get
			{
				return this.method.HasThis;
			}
			set
			{
				throw new InvalidOperationException();
			}
		}

		public override Mono.Cecil.MethodReturnType MethodReturnType
		{
			get
			{
				return this.method.MethodReturnType;
			}
			set
			{
				throw new InvalidOperationException();
			}
		}

		public override ModuleDefinition Module
		{
			get
			{
				return this.method.Module;
			}
		}

		public override string Name
		{
			get
			{
				return this.method.Name;
			}
			set
			{
				throw new InvalidOperationException();
			}
		}

		public override Collection<ParameterDefinition> Parameters
		{
			get
			{
				return this.method.Parameters;
			}
		}

		internal MethodSpecification(MethodReference method)
		{
			if (method == null)
			{
				throw new ArgumentNullException("method");
			}
			this.method = method;
			this.token = new Mono.Cecil.MetadataToken(Mono.Cecil.TokenType.MethodSpec);
		}

		public sealed override MethodReference GetElementMethod()
		{
			return this.method.GetElementMethod();
		}
	}
}