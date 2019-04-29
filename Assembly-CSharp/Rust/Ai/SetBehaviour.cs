using Apex.Serialization;
using System;

namespace Rust.Ai
{
	public sealed class SetBehaviour : BaseAction
	{
		[ApexSerialization]
		public BaseNpc.Behaviour Behaviour;

		[ApexSerialization]
		public float BusyTime;

		public SetBehaviour()
		{
		}

		public override void DoExecute(BaseContext c)
		{
			if (c.AIAgent.CurrentBehaviour == this.Behaviour)
			{
				return;
			}
			c.AIAgent.CurrentBehaviour = this.Behaviour;
			if (this.BusyTime > 0f)
			{
				c.AIAgent.SetBusyFor(this.BusyTime);
			}
		}
	}
}