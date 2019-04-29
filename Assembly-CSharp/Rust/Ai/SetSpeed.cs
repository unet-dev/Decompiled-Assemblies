using Apex.Serialization;
using System;

namespace Rust.Ai
{
	public class SetSpeed : BaseAction
	{
		[ApexSerialization(defaultValue=BaseNpc.SpeedEnum.StandStill)]
		public BaseNpc.SpeedEnum @value;

		public SetSpeed()
		{
		}

		public override void DoExecute(BaseContext c)
		{
			c.AIAgent.TargetSpeed = c.AIAgent.ToSpeed(this.@value);
			c.SetFact(BaseNpc.Facts.Speed, (byte)this.@value);
		}
	}
}