using Apex.Serialization;
using System;

namespace Rust.Ai
{
	public sealed class Crouch : BaseAction
	{
		[ApexSerialization]
		public bool crouch;

		public Crouch()
		{
		}

		public override void DoExecute(BaseContext ctx)
		{
			if (this.crouch)
			{
				NPCPlayerApex aIAgent = ctx.AIAgent as NPCPlayerApex;
				if (aIAgent != null)
				{
					aIAgent.modelState.SetFlag(ModelState.Flag.Ducked, this.crouch);
				}
			}
		}
	}
}