using Apex.Serialization;
using System;

namespace Rust.Ai
{
	public sealed class AllowBreaking : BaseAction
	{
		[ApexSerialization]
		public bool Allow;

		public AllowBreaking()
		{
		}

		public override void DoExecute(BaseContext c)
		{
			c.AIAgent.AutoBraking = this.Allow;
		}
	}
}