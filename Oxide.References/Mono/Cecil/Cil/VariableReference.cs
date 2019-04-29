using Mono.Cecil;
using System;

namespace Mono.Cecil.Cil
{
	public abstract class VariableReference
	{
		private string name;

		internal int index = -1;

		protected TypeReference variable_type;

		public int Index
		{
			get
			{
				return this.index;
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

		public TypeReference VariableType
		{
			get
			{
				return this.variable_type;
			}
			set
			{
				this.variable_type = value;
			}
		}

		internal VariableReference(TypeReference variable_type) : this(string.Empty, variable_type)
		{
		}

		internal VariableReference(string name, TypeReference variable_type)
		{
			this.name = name;
			this.variable_type = variable_type;
		}

		public abstract VariableDefinition Resolve();

		public override string ToString()
		{
			if (!string.IsNullOrEmpty(this.name))
			{
				return this.name;
			}
			if (this.index < 0)
			{
				return string.Empty;
			}
			return string.Concat("V_", this.index);
		}
	}
}