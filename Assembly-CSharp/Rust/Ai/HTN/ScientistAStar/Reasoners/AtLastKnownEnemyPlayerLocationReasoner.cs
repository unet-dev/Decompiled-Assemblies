using Rust.Ai.HTN;
using Rust.Ai.HTN.Reasoning;
using Rust.Ai.HTN.ScientistAStar;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Rust.Ai.HTN.ScientistAStar.Reasoners
{
	public class AtLastKnownEnemyPlayerLocationReasoner : INpcReasoner
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

		public AtLastKnownEnemyPlayerLocationReasoner()
		{
		}

		public void Tick(IHTNAgent npc, float deltaTime, float time)
		{
			ScientistAStarContext npcContext = npc.AiDomain.NpcContext as ScientistAStarContext;
			if (npcContext == null)
			{
				return;
			}
			if (npcContext.Memory.PrimaryKnownEnemyPlayer.PlayerInfo.Player != null)
			{
				Vector3 destination = ScientistAStarDomain.AStarNavigateToLastKnownLocationOfPrimaryEnemyPlayer.GetDestination(npcContext);
				BasePathNode closestToPoint = npcContext.Domain.Path.GetClosestToPoint(destination);
				if (closestToPoint != null && closestToPoint.transform != null && (closestToPoint.transform.position - npcContext.BodyPosition).sqrMagnitude < 1f)
				{
					npcContext.SetFact(Facts.AtLocationLastKnownLocationOfPrimaryEnemyPlayer, 1, true, true, true);
					return;
				}
			}
			npcContext.SetFact(Facts.AtLocationLastKnownLocationOfPrimaryEnemyPlayer, 0, true, true, true);
		}
	}
}