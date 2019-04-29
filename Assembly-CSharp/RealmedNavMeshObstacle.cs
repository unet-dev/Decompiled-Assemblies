using Rust.Ai;
using System;
using UnityEngine;
using UnityEngine.AI;

public class RealmedNavMeshObstacle : BasePrefab
{
	public NavMeshObstacle Obstacle;

	public RealmedNavMeshObstacle()
	{
	}

	public override void PreProcess(IPrefabProcessor process, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		if (bundling)
		{
			return;
		}
		base.PreProcess(process, rootObj, name, serverside, clientside, false);
		if (base.isServer && this.Obstacle)
		{
			if (AiManager.nav_disable)
			{
				process.RemoveComponent(this.Obstacle);
				this.Obstacle = null;
			}
			else if (AiManager.nav_obstacles_carve_state >= 2)
			{
				this.Obstacle.carving = true;
			}
			else if (AiManager.nav_obstacles_carve_state != 1)
			{
				this.Obstacle.carving = false;
			}
			else
			{
				this.Obstacle.carving = this.Obstacle.gameObject.layer == 21;
			}
		}
		process.RemoveComponent(this);
	}
}