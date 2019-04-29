using System;

namespace Mono.Cecil
{
	public abstract class ParameterReference : IMetadataTokenProvider
	{
		private string name;

		internal int index = -1;

		protected TypeReference parameter_type;

		internal Mono.Cecil.MetadataToken token;

		public int Index
		{
			get
			{
				return this.index;
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

		public TypeReference ParameterType
		{
			get
			{
				return this.parameter_type;
			}
			set
			{
				this.parameter_type = value;
			}
		}

		internal ParameterReference(string name, TypeReference parameterType)
		{
			if (parameterType == null)
			{
				throw new ArgumentNullException("parameterType");
			}
			this.name = name ?? string.Empty;
			this.parameter_type = parameterType;
		}

		public abstract ParameterDefinition Resolve();

		public override string ToString()
		{
			return this.name;
		}
	}
}