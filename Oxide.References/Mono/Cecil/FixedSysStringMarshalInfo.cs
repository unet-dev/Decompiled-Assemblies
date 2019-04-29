using System;

namespace Mono.Cecil
{
	public sealed class FixedSysStringMarshalInfo : MarshalInfo
	{
		internal int size;

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

		public FixedSysStringMarshalInfo() : base(Mono.Cecil.NativeType.FixedSysString)
		{
			this.size = -1;
		}
	}
}