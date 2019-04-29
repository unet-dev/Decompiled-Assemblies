using Rust.Ai.HTN;
using Rust.Ai.HTN.Reasoning;
using Rust.Ai.HTN.ScientistAStar;
using System;
using System.Runtime.CompilerServices;

namespace Rust.Ai.HTN.ScientistAStar.Reasoners
{
	public class AmmoReasoner : INpcReasoner
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

		public AmmoReasoner()
		{
		}

		public void Tick(IHTNAgent npc, float deltaTime, float time)
		{
			ScientistAStarContext npcContext = npc.AiDomain.NpcContext as ScientistAStarContext;
			if (npcContext == null)
			{
				return;
			}
			HTNPlayer hTNPlayer = npc as HTNPlayer;
			if (hTNPlayer == null)
			{
				return;
			}
			AttackEntity heldEntity = hTNPlayer.GetHeldEntity() as AttackEntity;
			if (heldEntity)
			{
				BaseProjectile baseProjectile = heldEntity as BaseProjectile;
				if (baseProjectile != null)
				{
					float single = (float)baseProjectile.primaryMagazine.contents / (float)baseProjectile.primaryMagazine.capacity;
					if (single > 0.9f)
					{
						npcContext.SetFact(Facts.AmmoState, AmmoState.FullClip, true, true, true);
						return;
					}
					if (single > 0.6f)
					{
						npcContext.SetFact(Facts.AmmoState, AmmoState.HighClip, true, true, true);
						return;
					}
					if (single > 0.17f)
					{
						npcContext.SetFact(Facts.AmmoState, AmmoState.MediumClip, true, true, true);
						return;
					}
					if (single > 0f)
					{
						npcContext.SetFact(Facts.AmmoState, AmmoState.LowAmmo, true, true, true);
						return;
					}
					npcContext.SetFact(Facts.AmmoState, AmmoState.EmptyClip, true, true, true);
					return;
				}
			}
			npcContext.SetFact(Facts.AmmoState, AmmoState.DontRequireAmmo, true, true, true);
		}
	}
}