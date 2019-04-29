using Apex.Serialization;
using System;

namespace Rust.Ai
{
	public class SetHumanFactBodyState : BaseAction
	{
		[ApexSerialization(defaultValue=NPCPlayerApex.BodyState.StandingTall)]
		public NPCPlayerApex.BodyState @value;

		public SetHumanFactBodyState()
		{
		}

		public override void DoExecute(BaseContext c)
		{
			c.SetFact(NPCPlayerApex.Facts.BodyState, (byte)this.@value, true, false);
		}
	}
}