using System;
using UnityEngine;

namespace Rust.Ai
{
	public class IsAtSpawnLocation : BaseScorer
	{
		public IsAtSpawnLocation()
		{
		}

		public static bool Evaluate(NPCHumanContext c)
		{
			if (!c.AIAgent.IsNavRunning())
			{
				return false;
			}
			Vector3 spawnPosition = c.Human.SpawnPosition - c.Position;
			return spawnPosition.sqrMagnitude < 4f;
		}

		public override float GetScore(BaseContext c)
		{
			object obj;
			if (IsAtSpawnLocation.Evaluate(c as NPCHumanContext))
			{
				obj = 1;
			}
			else
			{
				obj = null;
			}
			return (float)obj;
		}
	}
}