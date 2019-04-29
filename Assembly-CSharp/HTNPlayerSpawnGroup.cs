using Rust.Ai.HTN;
using System;
using UnityEngine;

public class HTNPlayerSpawnGroup : SpawnGroup
{
	[Header("HTN Player Spawn Group")]
	public HTNDomain.MovementRule Movement = HTNDomain.MovementRule.FreeMove;

	public float MovementRadius = -1f;

	public HTNPlayerSpawnGroup()
	{
	}

	protected override void PostSpawnProcess(BaseEntity entity, BaseSpawnPoint spawnPoint)
	{
		HTNPlayer movement = entity as HTNPlayer;
		if (movement != null && movement.AiDomain != null)
		{
			movement.AiDomain.Movement = this.Movement;
			movement.AiDomain.MovementRadius = this.MovementRadius;
		}
	}
}