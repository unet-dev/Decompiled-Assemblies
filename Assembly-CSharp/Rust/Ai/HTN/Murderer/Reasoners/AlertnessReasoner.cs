using Rust.Ai.HTN;
using Rust.Ai.HTN.Murderer;
using Rust.Ai.HTN.Reasoning;
using System;
using System.Runtime.CompilerServices;

namespace Rust.Ai.HTN.Murderer.Reasoners
{
	public class AlertnessReasoner : INpcReasoner
	{
		private float _lastFrustrationDecrementTime;

		public float LastTickTime
		{
			get;
			set;
		}

		public float TickFrequency
		{
			get;
			set;
		}

		public AlertnessReasoner()
		{
		}

		public void Tick(IHTNAgent npc, float deltaTime, float time)
		{
			MurdererContext npcContext = npc.AiDomain.NpcContext as MurdererContext;
			if (npcContext == null)
			{
				return;
			}
			if (npcContext.IsFact(Facts.Alertness))
			{
				if (npcContext.GetFact(Facts.Alertness) > 10)
				{
					npcContext.SetFact(Facts.Alertness, 10, true, false, true);
				}
				if (time - this._lastFrustrationDecrementTime > 1f)
				{
					this._lastFrustrationDecrementTime = time;
					npcContext.IncrementFact(Facts.Alertness, -1, true, true, true);
				}
			}
		}
	}
}