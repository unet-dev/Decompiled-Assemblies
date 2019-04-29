using Rust.Ai.HTN;
using Rust.Ai.HTN.Reasoning;
using Rust.Ai.HTN.ScientistAStar;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Rust.Ai.HTN.ScientistAStar.Reasoners
{
	public class EnemyPlayerLineOfSightReasoner : INpcReasoner
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

		public EnemyPlayerLineOfSightReasoner()
		{
		}

		public void Tick(IHTNAgent npc, float deltaTime, float time)
		{
			ScientistAStarContext npcContext = npc.AiDomain.NpcContext as ScientistAStarContext;
			if (npcContext == null)
			{
				return;
			}
			npcContext.SetFact(Facts.CanSeeEnemy, npcContext.EnemyPlayersInLineOfSight.Count > 0, true, true, true);
			float single = 0f;
			NpcPlayerInfo npcPlayerInfo = new NpcPlayerInfo();
			foreach (NpcPlayerInfo enemyPlayersInLineOfSight in npc.AiDomain.NpcContext.EnemyPlayersInLineOfSight)
			{
				float forwardDotDir = (enemyPlayersInLineOfSight.ForwardDotDir + 1f) * 0.5f;
				float sqrDistance = (1f - enemyPlayersInLineOfSight.SqrDistance / npc.AiDefinition.Engagement.SqrAggroRange) * 2f + forwardDotDir;
				if (sqrDistance > single)
				{
					single = sqrDistance;
					npcPlayerInfo = enemyPlayersInLineOfSight;
				}
				NpcPlayerInfo npcPlayerInfo1 = enemyPlayersInLineOfSight;
				npcPlayerInfo1.VisibilityScore = sqrDistance;
				npcContext.Memory.RememberEnemyPlayer(npc, ref npcPlayerInfo1, time, 0f, "SEE!");
			}
			npcContext.PrimaryEnemyPlayerInLineOfSight = npcPlayerInfo;
			if (npcPlayerInfo.Player != null && (npcContext.Memory.PrimaryKnownEnemyPlayer.PlayerInfo.Player == null || npcContext.Memory.PrimaryKnownEnemyPlayer.PlayerInfo.AudibleScore < single))
			{
				npcContext.Memory.RememberPrimaryEnemyPlayer(npcPlayerInfo.Player);
				npcContext.IncrementFact(Facts.Alertness, 2, true, true, true);
			}
		}
	}
}