using Rust.Ai.HTN;
using Rust.Ai.HTN.Reasoning;
using Rust.Ai.HTN.ScientistAStar;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Rust.Ai.HTN.ScientistAStar.Reasoners
{
	public class AtHomeLocationReasoner : INpcReasoner
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

		public AtHomeLocationReasoner()
		{
		}

		public void Tick(IHTNAgent npc, float deltaTime, float time)
		{
			ScientistAStarContext npcContext = npc.AiDomain.NpcContext as ScientistAStarContext;
			if (npcContext == null)
			{
				return;
			}
			if ((npcContext.BodyPosition - npcContext.Domain.SpawnPosition).sqrMagnitude < 3f)
			{
				npcContext.SetFact(Facts.AtLocationHome, true, true, true, true);
				return;
			}
			BasePathNode closestToPoint = npcContext.Domain.Path.GetClosestToPoint(npcContext.Domain.SpawnPosition);
			if (closestToPoint != null && closestToPoint.transform != null && (npcContext.BodyPosition - closestToPoint.transform.position).sqrMagnitude < 3f)
			{
				npcContext.SetFact(Facts.AtLocationHome, true, true, true, true);
			}
			npcContext.SetFact(Facts.AtLocationHome, false, true, true, true);
		}
	}
}