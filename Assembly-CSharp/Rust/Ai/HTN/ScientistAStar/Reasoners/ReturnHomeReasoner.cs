using Rust.Ai.HTN;
using Rust.Ai.HTN.Reasoning;
using Rust.Ai.HTN.ScientistAStar;
using System;
using System.Runtime.CompilerServices;

namespace Rust.Ai.HTN.ScientistAStar.Reasoners
{
	public class ReturnHomeReasoner : INpcReasoner
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

		public ReturnHomeReasoner()
		{
		}

		public void Tick(IHTNAgent npc, float deltaTime, float time)
		{
			ScientistAStarContext npcContext = npc.AiDomain.NpcContext as ScientistAStarContext;
			if (npcContext == null)
			{
				return;
			}
			if (!npcContext.IsFact(Facts.IsReturningHome))
			{
				if (npcContext.Memory.PrimaryKnownEnemyPlayer.PlayerInfo.Player == null || time - npcContext.Memory.PrimaryKnownEnemyPlayer.Time > npcContext.Body.AiDefinition.Memory.NoSeeReturnToSpawnTime)
				{
					npcContext.SetFact(Facts.IsReturningHome, true, true, true, true);
					return;
				}
			}
			else if (npcContext.IsFact(Facts.CanSeeEnemy) || time - npcContext.Body.lastAttackedTime < 2f || npcContext.IsFact(Facts.AtLocationHome))
			{
				npcContext.SetFact(Facts.IsReturningHome, false, true, true, true);
			}
		}
	}
}