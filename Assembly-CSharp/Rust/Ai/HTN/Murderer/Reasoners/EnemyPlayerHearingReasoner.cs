using Rust.Ai.HTN;
using Rust.Ai.HTN.Murderer;
using Rust.Ai.HTN.Reasoning;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Rust.Ai.HTN.Murderer.Reasoners
{
	public class EnemyPlayerHearingReasoner : INpcReasoner
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

		public EnemyPlayerHearingReasoner()
		{
		}

		public void Tick(IHTNAgent npc, float deltaTime, float time)
		{
			MurdererContext npcContext = npc.AiDomain.NpcContext as MurdererContext;
			if (npcContext == null)
			{
				return;
			}
			npcContext.SetFact(Facts.CanHearEnemy, npcContext.EnemyPlayersAudible.Count > 0, true, true, true);
			float single = 0f;
			NpcPlayerInfo npcPlayerInfo = new NpcPlayerInfo();
			foreach (NpcPlayerInfo enemyPlayersAudible in npc.AiDomain.NpcContext.EnemyPlayersAudible)
			{
				if (enemyPlayersAudible.SqrDistance > npc.AiDefinition.Sensory.SqrHearingRange)
				{
					continue;
				}
				float single1 = 1f - Mathf.Min(1f, enemyPlayersAudible.SqrDistance / npc.AiDefinition.Sensory.SqrHearingRange);
				float single2 = single1 * 2f;
				if (single2 > single)
				{
					single = single2;
					npcPlayerInfo = enemyPlayersAudible;
				}
				NpcPlayerInfo npcPlayerInfo1 = enemyPlayersAudible;
				npcPlayerInfo1.AudibleScore = single2;
				npcContext.Memory.RememberEnemyPlayer(npc, ref npcPlayerInfo1, time, (1f - single1) * 20f, "SOUND!");
			}
			npcContext.PrimaryEnemyPlayerAudible = npcPlayerInfo;
			if (npcPlayerInfo.Player != null && (npcContext.Memory.PrimaryKnownEnemyPlayer.PlayerInfo.Player == null || npcContext.Memory.PrimaryKnownEnemyPlayer.PlayerInfo.VisibilityScore < single))
			{
				npcContext.Memory.RememberPrimaryEnemyPlayer(npcPlayerInfo.Player);
				npcContext.IncrementFact(Facts.Alertness, 1, true, true, true);
			}
		}
	}
}