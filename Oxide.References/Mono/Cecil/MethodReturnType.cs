using Mono.Collections.Generic;
using System;
using System.Threading;

namespace Mono.Cecil
{
	public sealed class MethodReturnType : IConstantProvider, IMetadataTokenProvider, ICustomAttributeProvider, IMarshalInfoProvider
	{
		internal IMethodSignature method;

		internal ParameterDefinition parameter;

		private TypeReference return_type;

		public ParameterAttributes Attributes
		{
			get
			{
				return this.Parameter.Attributes;
			}
			set
			{
				this.Parameter.Attributes = value;
			}
		}

		public object Constant
		{
			get
			{
				return this.Parameter.Constant;
			}
			set
			{
				this.Parameter.Constant = value;
			}
		}

		public Collection<CustomAttribute> CustomAttributes
		{
			get
			{
				return this.Parameter.CustomAttributes;
			}
		}

		public bool HasConstant
		{
			get
			{
				if (this.parameter == null)
				{
					return false;
				}
				return this.parameter.HasConstant;
			}
			set
			{
				this.Parameter.HasConstant = value;
			}
		}

		public bool HasCustomAttributes
		{
			get
			{
				if (this.parameter == null)
				{
					return false;
				}
				return this.parameter.HasCustomAttributes;
			}
		}

		public bool HasDefault
		{
			get
			{
				if (this.parameter == null)
				{
					return false;
				}
				return this.parameter.HasDefault;
			}
			set
			{
				this.Parameter.HasDefault = value;
			}
		}

		public bool HasFieldMarshal
		{
			get
			{
				if (this.parameter == null)
				{
					return false;
				}
				return this.parameter.HasFieldMarshal;
			}
			set
			{
				this.Parameter.HasFieldMarshal = value;
			}
		}

		public bool HasMarshalInfo
		{
			get
			{
				if (this.parameter == null)
				{
					return false;
				}
				return this.parameter.HasMarshalInfo;
			}
		}

		public Mono.Cecil.MarshalInfo MarshalInfo
		{
			get
			{
				return this.Parameter.MarshalInfo;
			}
			set
			{
				this.Parameter.MarshalInfo = value;
			}
		}

		public Mono.Cecil.MetadataToken MetadataToken
		{
			get
			{
				return this.Parameter.MetadataToken;
			}
			set
			{
				this.Parameter.MetadataToken = value;
			}
		}

		public IMethodSignature Method
		{
			get
			{
				return this.method;
			}
		}

		internal ParameterDefinition Parameter
		{
			get
			{
				if (this.parameter == null)
				{
					Interlocked.CompareExchange<ParameterDefinition>(ref this.parameter, new ParameterDefinition(this.return_type, this.method), null);
				}
				return this.parameter;
			}
		}

		public TypeReference ReturnType
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

		public MethodReturnType(IMethodSignature method)
		{
			this.method = method;
		}
	}
}