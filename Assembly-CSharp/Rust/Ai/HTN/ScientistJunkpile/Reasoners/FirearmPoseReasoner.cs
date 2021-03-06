using Rust.Ai.HTN;
using Rust.Ai.HTN.Reasoning;
using Rust.Ai.HTN.ScientistJunkpile;
using System;
using System.Runtime.CompilerServices;

namespace Rust.Ai.HTN.ScientistJunkpile.Reasoners
{
	public class FirearmPoseReasoner : INpcReasoner
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

		public FirearmPoseReasoner()
		{
		}

		public void Tick(IHTNAgent npc, float deltaTime, float time)
		{
			ScientistJunkpileContext npcContext = npc.AiDomain.NpcContext as ScientistJunkpileContext;
			if (npcContext == null)
			{
				return;
			}
			HTNPlayer hTNPlayer = npc as HTNPlayer;
			if (hTNPlayer == null)
			{
				return;
			}
			if (npcContext.GetFact(Facts.FirearmOrder) == 0)
			{
				hTNPlayer.SetPlayerFlag(BasePlayer.PlayerFlags.Relaxed, true);
				return;
			}
			hTNPlayer.SetPlayerFlag(BasePlayer.PlayerFlags.Relaxed, false);
		}
	}
}