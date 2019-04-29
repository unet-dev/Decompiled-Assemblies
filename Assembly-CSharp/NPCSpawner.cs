using System;
using UnityEngine;

public class NPCSpawner : SpawnGroup
{
	public MonumentNavMesh monumentNavMesh;

	public bool shouldFillOnSpawn;

	public NPCSpawner()
	{
	}

	public void LateSpawn()
	{
		if (!this.WaitingForNavMesh())
		{
			this.SpawnInitial();
			Debug.Log("Navmesh complete, spawning");
			return;
		}
		base.Invoke(new Action(this.LateSpawn), 5f);
	}

	public override void SpawnInitial()
	{
		this.fillOnSpawn = this.shouldFillOnSpawn;
		if (!this.WaitingForNavMesh())
		{
			base.SpawnInitial();
			return;
		}
		base.Invoke(new Action(this.LateSpawn), 10f);
	}

	public bool WaitingForNavMesh()
	{
		if (this.monumentNavMesh == null)
		{
			return false;
		}
		return this.monumentNavMesh.IsBuilding;
	}
}