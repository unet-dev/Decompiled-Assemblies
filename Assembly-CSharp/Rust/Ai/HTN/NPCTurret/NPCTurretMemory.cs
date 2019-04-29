using Rust.Ai.HTN;
using System;

namespace Rust.Ai.HTN.NPCTurret
{
	[Serializable]
	public class NPCTurretMemory : BaseNpcMemory
	{
		[NonSerialized]
		public Rust.Ai.HTN.NPCTurret.NPCTurretContext NPCTurretContext;

		public override BaseNpcDefinition Definition
		{
			get
			{
				return this.NPCTurretContext.Body.AiDefinition;
			}
		}

		public NPCTurretMemory(Rust.Ai.HTN.NPCTurret.NPCTurretContext context) : base(context)
		{
			this.NPCTurretContext = context;
		}

		public override void ResetState()
		{
			base.ResetState();
		}
	}
}