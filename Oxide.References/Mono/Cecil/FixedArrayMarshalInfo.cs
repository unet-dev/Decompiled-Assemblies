using System;

namespace Mono.Cecil
{
	public sealed class FixedArrayMarshalInfo : MarshalInfo
	{
		internal Mono.Cecil.NativeType element_type;

		internal int size;

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

		public FixedArrayMarshalInfo() : base(Mono.Cecil.NativeType.FixedArray)
		{
			this.element_type = Mono.Cecil.NativeType.None;
		}
	}
}