using System;

namespace Mono.Cecil
{
	public sealed class ArrayMarshalInfo : MarshalInfo
	{
		internal Mono.Cecil.NativeType element_type;

		internal int size_parameter_index;

		internal int size;

		internal int size_parameter_multiplier;

		public Mono.Cecil.NativeType ElementType
		{
			get
			{
				return this.element_type;
			}
			set
			{
				this.element_type = value;
			}
		}

		public int Size
		{
			get
			{
				return this.size;
			}
			set
			{
				this.size = value;
			}
		}

		public int SizeParameterIndex
		{
			get
			{
				return this.size_parameter_index;
			}
			set
			{
				this.size_parameter_index = value;
			}
		}

		public int SizeParameterMultiplier
		{
			get
			{
				return this.size_parameter_multiplier;
			}
			set
			{
				this.size_parameter_multiplier = value;
			}
		}

		public ArrayMarshalInfo() : base(Mono.Cecil.NativeType.Array)
		{
			this.element_type = Mono.Cecil.NativeType.None;
			this.size_parameter_index = -1;
			this.size = -1;
			this.size_parameter_multiplier = -1;
		}
	}
}