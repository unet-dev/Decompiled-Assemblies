using Apex.Serialization;
using System;

namespace Rust.Ai
{
	public class SetBusyFor : BaseAction
	{
		[ApexSerialization]
		public float BusyTime;

		public SetBusyFor()
		{
		}

		public override void DoExecute(BaseContext c)
		{
			c.AIAgent.SetBusyFor(this.BusyTime);
		}
	}
}