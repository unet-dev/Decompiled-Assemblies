using Rust.Ai.HTN;
using Rust.Ai.HTN.Bear;
using Rust.Ai.HTN.Reasoning;
using System;
using System.Runtime.CompilerServices;

namespace Rust.Ai.HTN.Bear.Reasoners
{
	public class PreferredFightingRangeReasoner : INpcReasoner
	{
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

		public PreferredFightingRangeReasoner()
		{
		}

		public static bool IsAtPreferredRange(BearContext context, ref NpcPlayerInfo target)
		{
			return target.SqrDistance <= context.Body.AiDefinition.Engagement.SqrCloseRange;
		}

		public void Tick(IHTNAgent npc, float deltaTime, float time)
		{
			BearContext npcContext = npc.AiDomain.NpcContext as BearContext;
			if (npcContext == null)
			{
				return;
			}
			NpcPlayerInfo primaryEnemyPlayerTarget = npcContext.GetPrimaryEnemyPlayerTarget();
			if (primaryEnemyPlayerTarget.Player != null)
			{
				if (PreferredFightingRangeReasoner.IsAtPreferredRange(npcContext, ref primaryEnemyPlayerTarget))
				{
					npcContext.SetFact(Facts.AtLocationPreferredFightingRange, 1, true, true, true);
					return;
				}
				npcContext.SetFact(Facts.AtLocationPreferredFightingRange, 0, true, true, true);
			}
		}
	}
}