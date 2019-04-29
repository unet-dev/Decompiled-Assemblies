using System;

namespace Mono.Cecil
{
	public class MarshalInfo
	{
		internal Mono.Cecil.NativeType native;

		public Mono.Cecil.NativeType NativeType
		{
			get
			{
				return this.native;
			}
			set
			{
				this.native = value;
			}
		}

		public MarshalInfo(Mono.Cecil.NativeType native)
		{
			this.native = native;
		}
	}
}